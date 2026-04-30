import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { FinancialApiService } from '../../core/financial-api.service';
import { QuoteSnapshot, StockListItem, SyncWatchlistQuotesResult, Watchlist } from '../../shared/models/api.models';

@Component({
  selector: 'app-watchlist-detail-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './watchlist-detail.component.html',
  styleUrl: './watchlist-detail.component.sass',
})
export class WatchlistDetailComponent {
  private readonly api = inject(FinancialApiService);
  private readonly route = inject(ActivatedRoute);

  readonly watchlist = signal<Watchlist | null>(null);
  readonly catalog = signal<StockListItem[]>([]);
  readonly symbol = new FormControl('', { nonNullable: true, validators: [Validators.required] });
  readonly message = signal<string | null>(null);
  readonly loading = signal(true);
  readonly syncing = signal(false);
  readonly saving = signal(false);
  readonly syncSummary = signal<string | null>(null);
  readonly syncFailedSymbols = signal<string[]>([]);
  readonly syncedSnapshots = signal<QuoteSnapshot[]>([]);
  readonly hasStocks = computed(() => (this.watchlist()?.stocks.length ?? 0) > 0);
  readonly hasCatalog = computed(() => this.catalog().length > 0);
  readonly trackedSymbols = computed(() => new Set(this.watchlist()?.stocks.map((stock) => stock.symbol) ?? []));

  constructor() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.api.getStocks('', 1, 1000).subscribe((result) => this.catalog.set(result.items));
      this.loadWatchlist(id);
    }
  }

  addStock() {
    const current = this.watchlist();
    const symbol = this.symbol.value.trim().toUpperCase();
    if (!current || this.symbol.invalid || this.saving()) return;

    this.message.set(null);
    this.saving.set(true);
    this.api.addStockToWatchlist(current.id, symbol).subscribe({
      next: () => {
        this.symbol.reset('');
        this.message.set(`${symbol} added.`);
        this.saving.set(false);
        this.loadWatchlist(current.id);
      },
      error: () => {
        this.message.set(`Could not add ${symbol}.`);
        this.saving.set(false);
      },
    });
  }

  syncQuotes() {
    const current = this.watchlist();
    if (!current || this.syncing()) return;

    this.message.set('Syncing quotes...');
    this.syncing.set(true);
    this.api.syncWatchlistQuotes(current.id).subscribe({
      next: (result) => {
        this.applySyncResult(result);
        this.syncing.set(false);
        this.loadWatchlist(current.id);
      },
      error: () => {
        this.message.set('Quote sync failed.');
        this.syncing.set(false);
      },
    });
  }

  removeStock(symbol: string) {
    const current = this.watchlist();
    if (!current || this.saving()) return;

    this.message.set(null);
    this.saving.set(true);
    this.api.removeStockFromWatchlist(current.id, symbol).subscribe({
      next: () => {
        this.message.set(`${symbol} removed.`);
        this.saving.set(false);
        this.loadWatchlist(current.id);
      },
      error: () => {
        this.message.set(`Could not remove ${symbol}.`);
        this.saving.set(false);
      },
    });
  }

  filteredCatalog() {
    const term = this.symbol.value.trim().toLowerCase();
    const existing = this.trackedSymbols();

    return this.catalog().filter((item) => {
      if (existing.has(item.symbol)) return false;
      if (!term) return true;
      return [item.symbol, item.displaySymbol, item.description ?? '', item.currency ?? '', item.type ?? '']
        .join(' ')
        .toLowerCase()
        .includes(term);
    });
  }

  trackQuote(_: number, stock: Watchlist['stocks'][number]) {
    return stock.id;
  }

  private loadWatchlist(id: string) {
    this.loading.set(true);
    this.api.getWatchlist(id).subscribe({
      next: (watchlist) => {
        this.watchlist.set(watchlist);
        this.loading.set(false);
      },
      error: () => {
        this.watchlist.set(null);
        this.loading.set(false);
      },
    });
  }

  private applySyncResult(result: SyncWatchlistQuotesResult) {
    this.syncSummary.set(`Requested ${result.requestedCount}, stored ${result.storedCount}.`);
    this.syncFailedSymbols.set(result.failedSymbols);
    this.syncedSnapshots.set(result.snapshots);
    this.message.set(result.failedSymbols.length > 0 ? 'Quote sync completed with failures.' : 'Quote sync completed.');
  }

  quoteForSymbol(symbol: string) {
    const normalized = symbol.toUpperCase();
    return this.syncedSnapshots().find((snapshot) => snapshot.symbol.toUpperCase() === normalized) ?? null;
  }
}
