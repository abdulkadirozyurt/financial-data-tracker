using FinancialDataTracker.Business.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace FinancialDataTracker.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class QuotesController(IQuoteSnapshotService quoteSnapshotService) : ControllerBase
{
    [HttpPost("sync/watchlists/{watchlistId:guid}")]
    public async Task<IActionResult> SyncWatchlistQuotes(Guid watchlistId, CancellationToken cancellationToken)
    {
        var result = await quoteSnapshotService.SyncWatchlistQuotesAsync(watchlistId, cancellationToken);
        return Ok(result);
    }
}
