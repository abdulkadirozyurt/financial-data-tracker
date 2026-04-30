import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';

import { FinancialApiService } from '../../core/financial-api.service';

@Component({
  selector: 'app-create-watchlist-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule],
  templateUrl: './create-watchlist.component.html',
  styleUrl: './create-watchlist.component.sass',
})
export class CreateWatchlistComponent {
  private readonly api = inject(FinancialApiService);
  private readonly router = inject(Router);

  readonly name = new FormControl('', { nonNullable: true, validators: [Validators.required] });
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  submit() {
    if (this.name.invalid || this.loading()) return;

    this.loading.set(true);
    this.error.set(null);

    this.api.createWatchlist({ name: this.name.value.trim() }).subscribe({
      next: async (watchlist) => {
        this.loading.set(false);
        await this.router.navigate(['/watchlists', watchlist.id]);
      },
      error: (error) => {
        this.loading.set(false);
        this.error.set(this.api.getErrorMessage(error, 'Failed to create watchlist.'));
      },
    });
  }
}
