import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

import {
  CreateWatchlistRequest,
  PagedResult,
  StockListItem,
  SyncWatchlistQuotesResult,
  TopMover,
  Watchlist,
} from '../shared/models/api.models';

type ApiErrorResponse = {
  message?: string;
  detail?: string | null;
};

@Injectable({ providedIn: 'root' })
export class FinancialApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api';

  getStocks(search = '', pageNumber = 1, pageSize = 20) {
    return this.http.get<PagedResult<StockListItem>>(`${this.baseUrl}/stocks`, {
      params: { search, pageNumber, pageSize },
    });
  }

  getWatchlists() {
    return this.http.get<Watchlist[]>(`${this.baseUrl}/watchlists`);
  }

  getWatchlist(id: string) {
    return this.http.get<Watchlist>(`${this.baseUrl}/watchlists/${encodeURIComponent(id)}`);
  }

  createWatchlist(request: CreateWatchlistRequest) {
    return this.http.post<Watchlist>(`${this.baseUrl}/watchlists`, request);
  }

  addStockToWatchlist(watchlistId: string, symbol: string) {
    return this.http.post<Watchlist>(
      `${this.baseUrl}/watchlists/${encodeURIComponent(watchlistId)}/stocks/${encodeURIComponent(symbol)}`,
      {},
    );
  }

  removeStockFromWatchlist(watchlistId: string, symbol: string) {
    return this.http.delete<void>(`${this.baseUrl}/watchlists/${encodeURIComponent(watchlistId)}/stocks/${encodeURIComponent(symbol)}`);
  }

  syncWatchlistQuotes(watchlistId: string) {
    return this.http.post<SyncWatchlistQuotesResult>(
      `${this.baseUrl}/quotes/sync/watchlists/${encodeURIComponent(watchlistId)}`,
      {},
    );
  }

  getTopMovers(direction: 'gainers' | 'losers' = 'gainers', limit = 5) {
    return this.http.get<TopMover[]>(`${this.baseUrl}/analytics/top-movers`, {
      params: { direction, limit },
    });
  }

  getErrorMessage(error: unknown, fallback: string) {
    if (error instanceof HttpErrorResponse) {
      const payload = error.error;
      if (typeof payload === 'string' && payload.trim()) {
        return payload;
      }

      if (payload && typeof payload === 'object' && 'message' in payload) {
        const message = (payload as ApiErrorResponse).message;
        if (typeof message === 'string' && message.trim()) {
          return message;
        }
      }
    }

    return fallback;
  }
}
