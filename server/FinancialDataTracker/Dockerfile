FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY FinancialDataTracker.slnx ./
COPY src/FinancialDataTracker.Business/FinancialDataTracker.Business.csproj src/FinancialDataTracker.Business/
COPY src/FinancialDataTracker.Core/FinancialDataTracker.Core.csproj src/FinancialDataTracker.Core/
COPY src/FinancialDataTracker.DataAccess/FinancialDataTracker.DataAccess.csproj src/FinancialDataTracker.DataAccess/
COPY src/FinancialDataTracker.Entities/FinancialDataTracker.Entities.csproj src/FinancialDataTracker.Entities/
COPY src/FinancialDataTracker.WebAPI/FinancialDataTracker.WebAPI.csproj src/FinancialDataTracker.WebAPI/

RUN dotnet restore src/FinancialDataTracker.WebAPI/FinancialDataTracker.WebAPI.csproj

COPY . .

RUN dotnet publish src/FinancialDataTracker.WebAPI/FinancialDataTracker.WebAPI.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "FinancialDataTracker.WebAPI.dll"]
