import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import { FinancialApiService } from '../../core/financial-api.service';
import { Watchlist } from '../../shared/models/api.models';

@Component({
  selector: 'app-watchlist-list-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink],
  templateUrl: './watchlist-list.component.html',
  styleUrl: './watchlist-list.component.sass',
})
export class WatchlistListComponent {
  private readonly api = inject(FinancialApiService);
  readonly watchlists = signal<Watchlist[]>([]);
  readonly totalStocks = computed(() => this.watchlists().reduce((total, watchlist) => total + watchlist.stocks.length, 0));
  readonly hasWatchlists = computed(() => this.watchlists().length > 0);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  constructor() {
    this.api.getWatchlists().subscribe({
      next: (watchlists) => {
        this.watchlists.set(watchlists);
        this.loading.set(false);
      },
      error: (error) => {
        this.error.set(this.api.getErrorMessage(error, 'Failed to load watchlists.'));
        this.loading.set(false);
      },
    });
  }
}
