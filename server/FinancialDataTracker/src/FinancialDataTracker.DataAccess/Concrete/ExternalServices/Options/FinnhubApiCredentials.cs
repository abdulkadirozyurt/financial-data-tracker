using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialDataTracker.DataAccess.Concrete.ExternalServices.Options;

public sealed class FinnhubApiCredentials
{
    public string BaseUrl { get; set; } = default!;
    public string ApiKey { get; set; } = default!;
}