using Crypto.Data;
using Crypto.DTOs;
using Crypto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Crypto.Controllers;

[Route("api/wallet")]
[ApiController]
public class WalletsController : ControllerBase
{
    private readonly CryptoDbContext _context;

    public WalletsController(CryptoDbContext context)
    {
        _context = context;
    }

    [HttpGet("{userid}")]
    public async Task<IActionResult> GetWallet(int userid)
    {
        var wallet = await _context.Wallets
            .Include(w => w.Portfolio)
            .ThenInclude(pi => pi.Cryptocurrency)
            .FirstOrDefaultAsync(w => w.UserId == userid);

        if (wallet == null)
            return NotFound("Wallet not found.");

        var walletDto = new WalletDto
        {
            WalletId = wallet.Id,
            Balance = wallet.Balance,
            Portfolio = wallet.Portfolio.Select(pi => new PortfolioItemDto
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

        return Ok(walletDto);
    }

    [HttpDelete("{userid}")]
    public async Task<IActionResult> DeleteWallet(int userid)
    {
        var wallet = await _context.Wallets
            .Include(w => w.Portfolio)
            .FirstOrDefaultAsync(w => w.UserId == userid);

        if (wallet == null)
            return NotFound("Wallet not found.");

        _context.PortfolioItems.RemoveRange(wallet.Portfolio);
        _context.Wallets.Remove(wallet);

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateWalletBalance(int userId, WalletUpdateDto dto)
    {
        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.UserId == userId);

        if (wallet == null)
            return NotFound("Wallet not found.");

        
        double newBalance = wallet.Balance + dto.Amount;

        
        if (newBalance < 0)
            return BadRequest("Insufficient balance for this operation.");

        wallet.Balance = newBalance;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "Wallet balance updated successfully",
            NewBalance = wallet.Balance
        });
    }
}