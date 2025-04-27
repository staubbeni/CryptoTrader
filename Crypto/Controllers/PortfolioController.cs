using Crypto.Data;
using Crypto.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Crypto.Controllers;

[Route("api/portfolio")]
[ApiController]
public class PortfolioController : ControllerBase
{
    private readonly CryptoDbContext _context;

    public PortfolioController(CryptoDbContext context)
    {
        _context = context;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetPortfolio(int userId)
    {
        var wallet = await _context.Wallets
            .Include(w => w.Portfolio)
            .ThenInclude(pi => pi.Cryptocurrency)
            .FirstOrDefaultAsync(w => w.UserId == userId);

        if (wallet == null)
            return NotFound("Wallet not found.");

        var portfolioDto = new PortfolioResponseDto
        {
            WalletId = wallet.Id,
            Balance = wallet.Balance,
            PortfolioItems = wallet.Portfolio.Select(pi => new PortfolioDto
            {
                CryptocurrencyId = pi.CryptocurrencyId,
                CryptocurrencyName = pi.Cryptocurrency.Name,
                CryptocurrencySymbol = pi.Cryptocurrency.Symbol,
                Quantity = pi.Quantity,
                PurchasePrice = pi.PurchasePrice,
                CurrentPrice = pi.Cryptocurrency.CurrentPrice,
                CurrentValue = pi.Quantity * pi.Cryptocurrency.CurrentPrice
            }).ToList()
        };

        return Ok(portfolioDto);
    }
}