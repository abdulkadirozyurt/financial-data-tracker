using FinancialDataTracker.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialDataTracker.Entities.Concrete;

public sealed class Watchlist:Entity
{
    public string Name { get; set; }=default!;
    public List<Stock> Stocks { get; set; }
}
