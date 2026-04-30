using FinancialDataTracker.Business.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace FinancialDataTracker.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AnalyticsController(IAnalyticsService analyticsService) : ControllerBase
{
    [HttpGet("top-movers")]
    public async Task<IActionResult> GetTopMovers(
        [FromQuery] string direction = "gainers",
        [FromQuery] int limit = 5,
        CancellationToken cancellationToken = default)
    {
        var result = await analyticsService.GetTopMoversAsync(direction, limit, cancellationToken);
        return Ok(result);
    }
}
