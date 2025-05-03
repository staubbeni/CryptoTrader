using Crypto.DTOs;
using Crypto.Services;
using Microsoft.AspNetCore.Mvc;

namespace Crypto.Controllers;

[Route("api/profit")]
[ApiController]
public class ProfitController : ControllerBase
{
    private readonly IWalletService _walletService;

    public ProfitController(IWalletService walletService)
    {
        _walletService = walletService;
    }


    [HttpGet("{userId}")]
    public async Task<IActionResult> GetProfit(int userId)
    {
        try
        {
            var profit = await _walletService.GetProfitAsync(userId);
            return Ok(profit);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("details/{userId}")]
    public async Task<IActionResult> GetProfitDetails(int userId)
    {
        try
        {
            var details = await _walletService.GetProfitDetailsAsync(userId);
            return Ok(new
            {
                details.WalletId,
                details.Balance,
                TotalProfit = details.PortfolioItems.Sum(pi => pi.Profit),
                details.PortfolioItems
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}