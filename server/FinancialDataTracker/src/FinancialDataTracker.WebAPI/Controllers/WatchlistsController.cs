using FinancialDataTracker.Business.Abstract;
using FinancialDataTracker.Entities.Concrete.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FinancialDataTracker.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class WatchlistsController(IWatchlistService watchlistService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await watchlistService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await watchlistService.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWatchlistRequest request, CancellationToken cancellationToken)
    {
        var result = await watchlistService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },result);
    }

    [HttpPost("{id:guid}/stocks/{symbol}")]
    public async Task<IActionResult> AddStock(Guid id, string symbol, CancellationToken cancellationToken)
    {
        var result = await watchlistService.AddStockAsync(id, symbol, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}/stocks/{symbol}")]
    public async Task<IActionResult> RemoveStock(Guid id, string symbol, CancellationToken cancellationToken)
    {
        await watchlistService.RemoveStockAsync(id, symbol, cancellationToken);
        return NoContent();
    }

}
