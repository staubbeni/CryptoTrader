using Crypto.Data;
using Crypto.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Crypto.Controllers;

[Route("api/profit")]
[ApiController]
public class ProfitController : ControllerBase
{
    private readonly CryptoDbContext _context;

    public ProfitController(CryptoDbContext context)
    {
        _context = context;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetProfit(int userId)
    {
        var wallet = await _context.Wallets
            .Include(w => w.Portfolio)
            .ThenInclude(pi => pi.Cryptocurrency)
            .FirstOrDefaultAsync(w => w.UserId == userId);

        if (wallet == null)
            return NotFound("Wallet not found.");

        double totalProfit = wallet.Portfolio.Sum(pi =>
            (pi.Cryptocurrency.CurrentPrice - pi.PurchasePrice) * pi.Quantity);

        totalProfit = Math.Round(totalProfit, 2);

        var response = new ProfitResponseDto
        {
            UserId = userId,
            TotalProfit = totalProfit
        };

        return Ok(response);
    }

    [HttpGet("details/{userId}")]
    public async Task<IActionResult> GetProfitDetails(int userId)
    {
        var wallet = await _context.Wallets
            .Include(w => w.Portfolio)
            .ThenInclude(pi => pi.Cryptocurrency)
            .FirstOrDefaultAsync(w => w.UserId == userId);

        if (wallet == null)
            return NotFound("Wallet not found.");

        var portfolioItems = wallet.Portfolio.Select(pi => new PortfolioDto
        {
            CryptocurrencyId = pi.CryptocurrencyId,
            CryptocurrencyName = pi.Cryptocurrency.Name,
            CryptocurrencySymbol = pi.Cryptocurrency.Symbol,
            Quantity = pi.Quantity,
            PurchasePrice = pi.PurchasePrice,
            CurrentPrice = pi.Cryptocurrency.CurrentPrice,
            CurrentValue = Math.Round(pi.Quantity * pi.Cryptocurrency.CurrentPrice, 2),
            Profit = Math.Round((pi.Cryptocurrency.CurrentPrice - pi.PurchasePrice) * pi.Quantity, 2)
        }).ToList();

        double totalProfit = portfolioItems.Sum(pi => pi.Profit);

        var response = new PortfolioResponseDto
        {
            WalletId = wallet.Id,
            Balance = wallet.Balance,
            PortfolioItems = portfolioItems
        };

        return Ok(new
        {
            response.WalletId,
            response.Balance,
            TotalProfit = totalProfit,
            response.PortfolioItems
        });
    }
}