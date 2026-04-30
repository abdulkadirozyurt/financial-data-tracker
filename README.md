# Financial Data Tracker

Financial Data Tracker is a layered ASP.NET Core Web API for tracking stocks, watchlists, quote snapshots, and simple market movement analytics.

The application synchronizes stock market data from Finnhub, stores it in SQL Server with Entity Framework Core, and exposes REST endpoints for stock search, watchlist management, quote snapshot synchronization, and top mover analysis.

## Core Capabilities

- US stock catalog synchronization from Finnhub
- Persisted stock, watchlist, and quote snapshot data
- Watchlist creation and stock membership management
- Quote snapshot synchronization for watchlists
- Automatic quote snapshot refresh through a hosted background service
- Top gainers / top losers analytics based on latest stored snapshots
- Swagger / OpenAPI support in Development mode

## Tech Stack

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Finnhub API
- Swagger / OpenAPI

## Architecture

The backend is separated into focused layers:

- `FinancialDataTracker.WebAPI`: controllers, middleware, hosted background services
- `FinancialDataTracker.Business`: application services and use case orchestration
- `FinancialDataTracker.DataAccess`: EF Core repositories, database context, external API client
- `FinancialDataTracker.Entities`: domain entities and API DTOs
- `FinancialDataTracker.Core`: shared abstractions and exception types

Controllers delegate work to business services. Business services coordinate repositories and external API services. Repositories isolate EF Core query and persistence details.

## External API

The project uses Finnhub as the financial data provider.

Finnhub integration currently covers:

- stock symbol catalog retrieval through `/stock/symbol`
- stock quote retrieval through `/quote`

Stock catalog data is synchronized by a hosted service. Quote data is stored as snapshots and can be synchronized manually through the API or automatically by the quote snapshot hosted service.

## Database

The project uses SQL Server with Entity Framework Core.

Main persisted entities:

- `Stock`
- `Watchlist`
- `StockQuoteSnapshot`

`Stock` stores catalog-level information such as symbol, display symbol, description, currency, and type. `Watchlist` stores user-defined stock groups. `StockQuoteSnapshot` stores point-in-time quote data used by analytics.

## Background Services

`StockSyncHostedService` synchronizes the stock catalog from Finnhub.

`QuoteSnapshotSyncHostedService` refreshes quote snapshots for watchlists that contain at least one stock. It starts shortly after application startup and repeats on a fixed interval.

## REST API

### Stocks

- `GET /api/stocks?search=AAPL&pageNumber=1&pageSize=20`

### Watchlists

- `GET /api/watchlists`
- `GET /api/watchlists/{id}`
- `POST /api/watchlists`
- `POST /api/watchlists/{id}/stocks/{symbol}`
- `DELETE /api/watchlists/{id}/stocks/{symbol}`

### Quotes

- `POST /api/quotes/sync/watchlists/{watchlistId}`

### Analytics

- `GET /api/analytics/top-movers?direction=gainers&limit=5`
- `GET /api/analytics/top-movers?direction=losers&limit=5`

## Configuration

The API expects a SQL Server connection string and Finnhub API credentials.

Configuration keys:

- `ConnectionStrings:SqlServer`
- `FinnhubApiCredentials:BaseUrl`
- `FinnhubApiCredentials:ApiKey`

Swagger and OpenAPI mappings are enabled in Development mode.

## Running With Docker

The full stack can be started from the repository root with Docker Compose. The compose setup includes SQL Server, the ASP.NET Core API, and the Angular client.

PowerShell:

```powershell
$env:FINNHUB_API_KEY="YOUR_API_KEY"
docker compose up --build -d
```

Bash / zsh:

```bash
export FINNHUB_API_KEY="YOUR_API_KEY"
docker compose up --build -d
```

Windows CMD:

```cmd
set FINNHUB_API_KEY=YOUR_API_KEY
docker compose up --build -d
```

After startup:

- Client: `http://localhost:4200`
- API: `http://localhost:5250`
- Swagger: `http://localhost:5250/swagger`

## Design Pattern

The project uses the Repository Pattern for database access.

Repository abstractions keep EF Core details outside controllers and business services. Shared CRUD-style operations live in a generic repository, while entity-specific queries are implemented in specialized repositories such as stock, watchlist, and quote snapshot repositories.

This keeps persistence concerns isolated and lets business services focus on application use cases.

## Current Scope

The backend currently focuses on stock catalog data, watchlists, quote snapshots, and top mover analytics.
