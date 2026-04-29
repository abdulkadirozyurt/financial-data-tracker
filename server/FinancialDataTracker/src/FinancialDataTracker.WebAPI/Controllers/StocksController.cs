using FinancialDataTracker.Business.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace FinancialDataTracker.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StocksController(IStockService stockService) : ControllerBase
{    
    [HttpGet]
    public async Task<IActionResult> GetStockData(
        [FromQuery] string? search,
        [FromQuery] int pageNumber=1,
        [FromQuery] int pageSize=20,
        CancellationToken cancellationToken = default)
    {
        var result = await stockService.GetStockListAsync(search, pageNumber, pageSize, cancellationToken);
        return Ok(result);
    }
}
