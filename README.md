# Financial Data Tracker

## Purpose

Financial Data Tracker is a small internal finance tool for tracking stock watchlists and quote snapshots.

The MVP goal is:

- sync a US stock catalog from Finnhub
- persist stock and watchlist data in SQL Server
- expose REST endpoints through ASP.NET Core Web API
- store quote snapshots for selected stocks
- provide a simple analytical view such as top gainers / top losers

This repository is currently being completed incrementally. The stock listing flow is implemented first, while sync, watchlist, quote snapshot, and analytics flows are still in progress.

## Tech Stack

- .NET 10 Web API
- Entity Framework Core
- SQL Server
- Finnhub API
- Swagger / OpenAPI

Solution structure:

- `server/FinancialDataTracker/src/FinancialDataTracker.WebAPI`
- `server/FinancialDataTracker/src/FinancialDataTracker.Business`
- `server/FinancialDataTracker/src/FinancialDataTracker.DataAccess`
- `server/FinancialDataTracker/src/FinancialDataTracker.Entities`
- `server/FinancialDataTracker/src/FinancialDataTracker.Core`

## Why Finnhub?

Finnhub was chosen because it provides a practical free tier and exposes the two endpoints that fit this MVP best:

- stock catalog data
- quote data

For this project, the intended Finnhub usage is:

- `/stock/symbol` for syncing stock symbols
- `/quote` for getting quote snapshots

This keeps the MVP small, realistic, and aligned with the assessment requirements.

## MVP Features

Target MVP features:

- sync US stock catalog from Finnhub
- search stored stocks
- create watchlists
- add and remove stocks from watchlists
- sync quote snapshots for a watchlist
- view top gainers and top losers from the latest stored snapshots

Current implementation status:

- project structure exists
- Entity Framework Core context exists
- Swagger is configured
- repository pattern is implemented for stock listing
- business service mapping for paged stock listing is implemented
- `GET /api/stocks` returns paged stock data
- watchlist, quote sync, and analytics flows are still incomplete

## Design Pattern

This project uses the Repository Pattern to isolate EF Core query details from business services.

The reason for this choice is straightforward:

- controllers should not know how EF queries are written
- business services should express use cases, not persistence details
- database access should be centralized and reusable

The repository implementation contains an inline comment that explicitly marks where the pattern is being applied, matching the assessment requirement.

## Setup

### Prerequisites

- .NET 10 SDK
- SQL Server or LocalDB
- a Finnhub API key

### Local configuration

Do not store real secrets in tracked files.

Use `appsettings.Development.json`, user-secrets, or environment variables for local values.

Example `appsettings.json` placeholders:

```json
{
  "ConnectionStrings": {
    "SqlServer": "Server=localhost;Database=FinancialDataTrackerDb;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
  },
  "FinnhubApiCredentials": {
    "BaseUrl": "https://finnhub.io/api/v1",
    "ApiKey": "YOUR_FINNHUB_API_KEY"
  }
}
```

Example user-secrets commands:

```powershell
dotnet user-secrets set "FinnhubApiCredentials:ApiKey" "YOUR_KEY" --project .\server\FinancialDataTracker\src\FinancialDataTracker.WebAPI
dotnet user-secrets set "ConnectionStrings:SqlServer" "YOUR_CONNECTION_STRING" --project .\server\FinancialDataTracker\src\FinancialDataTracker.WebAPI
```

Example environment variables:

```powershell
$env:FinnhubApiCredentials__ApiKey="YOUR_KEY"
$env:ConnectionStrings__SqlServer="YOUR_CONNECTION_STRING"
```

## Run

Restore and build:

```powershell
cd .\server\FinancialDataTracker
dotnet restore
dotnet build .\FinancialDataTracker.slnx
```

Apply database migrations:

```powershell
dotnet ef database update --project .\src\FinancialDataTracker.DataAccess --startup-project .\src\FinancialDataTracker.WebAPI
```

Run the API:

```powershell
dotnet run --project .\src\FinancialDataTracker.WebAPI
```

Swagger should be available when running in Development mode.

## API Endpoints

### Current endpoints in the repository

- `GET /api/stocks?search=AAPL&pageNumber=1&pageSize=20`
- `POST /api/watchlists/add`

Current behavior:

- `GET /api/stocks` returns paged stock list data through `IStockService` and `IStockRepository`
- `POST /api/watchlists/add` is still a placeholder

### Intended MVP endpoints

- `POST /api/stocks/sync?exchange=US`
- `GET /api/stocks?search=AAPL&page=1&pageSize=10`
- `POST /api/watchlists`
- `GET /api/watchlists/{id}`
- `POST /api/watchlists/{id}/stocks/{symbol}`
- `DELETE /api/watchlists/{id}/stocks/{symbol}`
- `POST /api/quotes/sync/watchlists/{watchlistId}`
- `GET /api/analytics/top-movers?direction=gainers&limit=5`
- `GET /api/analytics/top-movers?direction=losers&limit=5`

## Database Choice

SQL Server is used for this project.

Reason:

- it already fits the existing EF Core setup
- it supports the current migration flow cleanly
- it is a practical choice for a layered .NET backend project

Core persisted entities are intended to include:

- `Stock`
- `Watchlist`
- `StockQuoteSnapshot`

## Trade-offs

- no authentication is included in the MVP scope
- portfolio quantity / P&L tracking is intentionally out of scope
- quote sync is intended to be manually triggered so API usage stays predictable
- the frontend is planned after the backend MVP is stabilized
- some endpoints listed above are target endpoints and are not fully implemented yet
- controller and sync behavior still need to be aligned with the final MVP flow
