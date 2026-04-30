import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';

import { FinancialApiService } from '../../core/financial-api.service';
import { TopMover } from '../../shared/models/api.models';

@Component({
  selector: 'app-analytics-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './analytics.component.html',
  styleUrl: './analytics.component.sass',
})
export class AnalyticsComponent {
  private readonly api = inject(FinancialApiService);

  readonly direction = signal<'gainers' | 'losers'>('gainers');
  readonly movers = signal<TopMover[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly hasMovers = computed(() => this.movers().length > 0);

  constructor() {
    this.load();
  }

  setDirection(direction: 'gainers' | 'losers') {
    if (this.direction() === direction) return;
    this.direction.set(direction);
    this.load();
  }

  private load() {
    this.loading.set(true);
    this.error.set(null);
    this.api.getTopMovers(this.direction(), 8).subscribe({
      next: (movers) => {
        this.movers.set(movers);
        this.loading.set(false);
      },
      error: (error) => {
        this.movers.set([]);
        this.error.set(this.api.getErrorMessage(error, 'Analytics is unavailable right now. Sync quotes first and try again.'));
        this.loading.set(false);
      },
    });
  }
}
