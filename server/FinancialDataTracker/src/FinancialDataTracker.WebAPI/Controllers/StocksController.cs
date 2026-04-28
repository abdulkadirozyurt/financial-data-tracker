using FinancialDataTracker.Business.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace FinancialDataTracker.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StocksController(IStockService stockService) : ControllerBase
{
    private readonly IStockService _stockService = stockService;

    [HttpGet]
    public async Task<IActionResult> GetStockData()
    {

        await _stockService.WriteDatabaseAsync();
        return Ok("Stock data will be here");
    }
}
