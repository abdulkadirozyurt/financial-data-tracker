import { ChangeDetectionStrategy, Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormControl } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { FinancialApiService } from '../../core/financial-api.service';
import { PaginationComponent } from '../../shared/components/pagination.component';
import { PagedResult, StockListItem, Watchlist } from '../../shared/models/api.models';

@Component({
  selector: 'app-stocks-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule, PaginationComponent],
  templateUrl: './stocks.component.html',
  styleUrl: './stocks.component.sass',
})
export class StocksComponent {
  private readonly api = inject(FinancialApiService);
  private readonly destroyRef = inject(DestroyRef);

  readonly search = new FormControl('', { nonNullable: true });
  readonly pageNumber = signal(1);
  readonly pageSize = signal(20);
  readonly stocks = signal<PagedResult<StockListItem> | null>(null);
  readonly watchlists = signal<Watchlist[]>([]);
  readonly selectedWatchlistId = signal('');
  readonly loading = signal(true);
  readonly loadingWatchlists = signal(true);
  readonly addingSymbol = signal<string | null>(null);
  readonly error = signal<string | null>(null);
  readonly actionMessage = signal<string | null>(null);
  readonly currentStocksCount = computed(() => this.stocks()?.items.length ?? 0);
  readonly totalStocksCount = computed(() => this.stocks()?.totalCount ?? 0);
  readonly hasWatchlists = computed(() => this.watchlists().length > 0);
  readonly stocksPage = computed(() => this.stocks()?.page ?? 1);
  readonly stocksPageSize = computed(() => this.stocks()?.pageSize ?? this.pageSize());
  readonly stockItems = computed(() => this.stocks()?.items ?? []);

  constructor() {
    this.loadWatchlists();

    this.search.valueChanges.pipe(debounceTime(250), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef)).subscribe(() => {
      this.pageNumber.set(1);
      this.actionMessage.set(null);
      this.loadStocks();
    });

    this.loadStocks();
  }

  onPageChange(page: number) {
    this.pageNumber.set(Math.max(1, page));
    this.loadStocks();
  }

  onWatchlistChange(value: string) {
    this.selectedWatchlistId.set(value);
  }

  addStock(stock: StockListItem) {
    const watchlistId = this.selectedWatchlistId();
    if (!watchlistId || this.addingSymbol()) {
      this.actionMessage.set(!this.hasWatchlists() ? 'Create a watchlist first.' : 'Another stock is being added.');
      return;
    }

    this.addingSymbol.set(stock.symbol);
    this.actionMessage.set(`Adding ${stock.symbol} to watchlist...`);
    this.api.addStockToWatchlist(watchlistId, stock.symbol).subscribe({
      next: () => {
        this.actionMessage.set(`${stock.symbol} added to watchlist.`);
        this.addingSymbol.set(null);
        this.loadWatchlists();
      },
      error: (error) => {
        this.actionMessage.set(this.api.getErrorMessage(error, `Could not add ${stock.symbol} to the watchlist.`));
        this.addingSymbol.set(null);
      },
    });
  }

  trackById(_: number, item: StockListItem) {
    return item.id;
  }

  private loadStocks() {
    this.loading.set(true);
    this.error.set(null);

    this.api.getStocks(this.search.value.trim(), this.pageNumber(), this.pageSize()).subscribe({
      next: (result) => {
        this.stocks.set(result);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load stocks.');
        this.loading.set(false);
      },
    });
  }

  private loadWatchlists() {
    this.loadingWatchlists.set(true);

    this.api.getWatchlists().subscribe({
      next: (watchlists) => {
        this.watchlists.set(watchlists);
        if (!this.selectedWatchlistId() && watchlists.length > 0) {
          this.selectedWatchlistId.set(watchlists[0].id);
        }
        if (this.selectedWatchlistId() && !watchlists.some((watchlist) => watchlist.id === this.selectedWatchlistId()) && watchlists.length > 0) {
          this.selectedWatchlistId.set(watchlists[0].id);
        }
        this.loadingWatchlists.set(false);
      },
      error: () => {
        this.watchlists.set([]);
        this.loadingWatchlists.set(false);
      },
    });
  }
}
