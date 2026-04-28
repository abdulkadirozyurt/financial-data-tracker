using System;
using System.Collections.Generic;
using System.Text;
using FinancialDataTracker.Business.Abstract;
using FinancialDataTracker.DataAccess.Abstract;
using FinancialDataTracker.DataAccess.Concrete;

namespace FinancialDataTracker.Business.Concrete;

public sealed class StockManager(IStockDal stockDal): IStockService
{
    private readonly IStockDal _stockDal = stockDal;

    
     public async Task WriteDatabaseAsync()
    {
        await _stockDal.WriteDatabaseAsync();
    }
}
