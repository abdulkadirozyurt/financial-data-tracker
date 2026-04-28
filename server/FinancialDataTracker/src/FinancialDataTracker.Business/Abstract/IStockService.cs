using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialDataTracker.Business.Abstract;

public interface IStockService
{
    Task WriteDatabaseAsync();
}
