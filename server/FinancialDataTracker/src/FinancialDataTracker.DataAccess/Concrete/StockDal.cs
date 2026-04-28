using System;
using System.Collections.Generic;
using System.Text;
using FinancialDataTracker.DataAccess.Abstract;
using FinancialDataTracker.DataAccess.Abstract.ExternalServices;
using FinancialDataTracker.DataAccess.Concrete.Context;
using FinancialDataTracker.DataAccess.Concrete.ExternalServices;

namespace FinancialDataTracker.DataAccess.Concrete;

public sealed class StockDal(IFinnhubService finnhubService): IStockDal
{
    private readonly IFinnhubService _finnhubService = finnhubService;

    

    public async Task WriteDatabaseAsync()
    {
        await _finnhubService.WriteDatabaseAsync();
    }


}
