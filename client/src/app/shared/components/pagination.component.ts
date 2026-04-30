import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';

@Component({
  selector: 'app-pagination',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <nav class="pagination" aria-label="Pagination">
      <button type="button" class="pagination__button" (click)="pageChange.emit(currentPage() - 1)" [disabled]="currentPage() <= 1">Previous</button>
      <span class="pagination__status">Page {{ currentPage() }} of {{ totalPages() }}</span>
      <button type="button" class="pagination__button" (click)="pageChange.emit(currentPage() + 1)" [disabled]="currentPage() >= totalPages()">Next</button>
    </nav>
  `,
  styleUrl: './pagination.component.sass',
})
export class PaginationComponent {
  readonly currentPage = input(1);
  readonly pageSize = input(20);
  readonly totalCount = input(0);
  readonly pageChange = output<number>();

  readonly totalPages = computed(() => Math.max(1, Math.ceil(this.totalCount() / this.pageSize())));
}
