# Financial Data Tracker

## Purpose

Financial Data Tracker is a small internal finance tool built for the Rasyonet backend case study.

The current product direction is a stock watchlist workflow:

- sync a US stock catalog from Finnhub
- persist stock and watchlist data in SQL Server
- expose REST endpoints through ASP.NET Core Web API
- let users create watchlists and attach tracked stocks

The intended end state of the MVP is to also store quote snapshots and expose a simple analytics view such as top gainers / top losers.

## API Choice

This project uses **Finnhub** as the external financial data source.

Reason:

- practical free tier for an internship case study
- simple stock symbol catalog endpoint
- simple quote endpoint
- enough scope for a small internal finance tool without overbuilding

Current Finnhub usage in the codebase:

- `/stock/symbol` for stock catalog synchronization
- `/quote` support exists in the client service layer, but quote snapshot API flow is not finished yet

## Database Choice

This project uses **SQL Server** with **Entity Framework Core**.

Reason:

- it fits the current layered .NET backend structure well
- EF Core migrations are already in place
- stock, watchlist, and quote snapshot entities map naturally to a relational model

Core persisted entities:

- `Stock`
- `Watchlist`
- `StockQuoteSnapshot`

## Tech Stack

- .NET 10 Web API
- ASP.NET Core
- Entity Framework Core
- SQL Server
- Finnhub API
- Swagger / OpenAPI

Backend solution structure:

- `server/FinancialDataTracker/src/FinancialDataTracker.WebAPI`
- `server/FinancialDataTracker/src/FinancialDataTracker.Business`
- `server/FinancialDataTracker/src/FinancialDataTracker.DataAccess`
- `server/FinancialDataTracker/src/FinancialDataTracker.Entities`
- `server/FinancialDataTracker/src/FinancialDataTracker.Core`

## Design Pattern

This project uses the **Repository Pattern**.

Purpose:

- isolate EF Core query details from business services
- keep controllers free of persistence logic
- make business services express use cases instead of database access details

The repository implementation contains an inline comment explicitly naming the pattern, which was a requirement in the case study.

## Current Status

Implemented backend capabilities:

- layered project structure
- SQL Server persistence with EF Core migrations
- stock catalog synchronization service using Finnhub
- paged stock listing endpoint
- watchlist create/read/add/remove service flow
- watchlist controller endpoints
- Swagger wiring
- exception middleware

Still incomplete:

- quote snapshot sync endpoint
- analytics/top movers endpoint
- frontend integration
- final cleanup of some warnings and middleware polish

## Setup

### Prerequisites

- .NET 10 SDK
- SQL Server or LocalDB
- a Finnhub API key

### Local Configuration

Do not commit real secrets.

Use `appsettings.Development.json`, `user-secrets`, or environment variables for local values.

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

Example `user-secrets` commands:

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

Swagger / OpenAPI is configured for Development mode.

Important note:

- `Program.cs` currently maps controllers only inside the Development block.
- That means local development is the expected run mode right now.

## Current API

Implemented endpoints in the current repository:

- `GET /api/stocks?search=AAPL&pageNumber=1&pageSize=20`
- `GET /api/watchlists`
- `GET /api/watchlists/{id}`
- `POST /api/watchlists`
- `POST /api/watchlists/{id}/stocks/{symbol}`
- `DELETE /api/watchlists/{id}/stocks/{symbol}`

Behavior summary:

- `GET /api/stocks` returns paged stock data from persisted records
- watchlist endpoints allow create, read, add stock, and remove stock flows

## Planned API

These are part of the intended MVP but are not finished yet:

- `POST /api/stocks/sync?exchange=US` as a manual sync endpoint
- `POST /api/quotes/sync/watchlists/{watchlistId}`
- `GET /api/analytics/top-movers?direction=gainers&limit=5`
- `GET /api/analytics/top-movers?direction=losers&limit=5`

At the moment, stock catalog sync is driven by the hosted service rather than a manual controller endpoint.

## Trade-offs

- authentication is intentionally out of scope
- portfolio quantity / P&L tracking is out of scope
- quote snapshot and analytics flows are planned but not finished yet
- the backend is the priority; the Angular client is bonus scope
- some infrastructure is functional but still needs cleanup before final submission polish

This is intentionally being kept as a small, focused internal tool rather than a broad financial platform.
