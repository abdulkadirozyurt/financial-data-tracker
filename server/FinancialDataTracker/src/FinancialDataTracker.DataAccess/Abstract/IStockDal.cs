using System;
using System.Collections.Generic;
using System.Text;
using FinancialDataTracker.DataAccess.Concrete.ExternalServices;

namespace FinancialDataTracker.DataAccess.Abstract;

public interface IStockDal
{
    

    Task WriteDatabaseAsync();
    
}
