import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'stocks' },
  {
    path: 'stocks',
    loadComponent: () => import('./features/stocks/stocks.component').then((m) => m.StocksComponent),
  },
  {
    path: 'analytics',
    loadComponent: () =>
      import('./features/analytics/analytics.component').then((m) => m.AnalyticsComponent),
  },
  {
    path: 'watchlists',
    loadComponent: () =>
      import('./features/watchlists/watchlist-list.component').then((m) => m.WatchlistListComponent),
  },
  {
    path: 'watchlists/new',
    loadComponent: () =>
      import('./features/watchlists/create-watchlist.component').then((m) => m.CreateWatchlistComponent),
  },
  {
    path: 'watchlists/:id',
    loadComponent: () =>
      import('./features/watchlists/watchlist-detail.component').then((m) => m.WatchlistDetailComponent),
  },
  { path: '**', redirectTo: 'stocks' },
];
