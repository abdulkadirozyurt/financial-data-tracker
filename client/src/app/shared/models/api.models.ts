export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
}

export interface StockListItem {
  id: string;
  symbol: string;
  displaySymbol: string;
  description?: string | null;
  currency?: string | null;
  type?: string | null;
}

export interface QuoteSnapshot {
  id: string;
  symbol: string;
  currentPrice: number;
  openPrice: number;
  highPrice: number;
  lowPrice: number;
  previousClosePrice: number;
  change: number;
  percentChange: number;
  finnhubTimestampUtc?: string | null;
  fetchedAtUtc: string;
}

export interface WatchlistStock {
  id: string;
  symbol: string;
  displaySymbol: string;
  description?: string | null;
  currency?: string | null;
  type?: string | null;
}

export interface Watchlist {
  id: string;
  name: string;
  stocks: WatchlistStock[];
}

export interface CreateWatchlistRequest {
  name: string;
}

export interface TopMover {
  symbol: string;
  displaySymbol: string;
  description?: string | null;
  currentPrice: number;
  previousClosePrice: number;
  change: number;
  percentChange: number;
  fetchedAtUtc: string;
}

export interface SyncWatchlistQuotesResult {
  watchlistId: string;
  watchlistName: string;
  requestedCount: number;
  storedCount: number;
  failedSymbols: string[];
  snapshots: QuoteSnapshot[];
}
