using Crypto.Data;
using Crypto.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Crypto.Services;

public interface IPortfolioService
{
    Task<PortfolioResponseDto> GetPortfolioAsync(int userId);
}

public class PortfolioService : IPortfolioService
{
    private readonly CryptoDbContext _context;

    public PortfolioService(CryptoDbContext context)
    {
        _context = context;
    }

    public async Task<PortfolioResponseDto> GetPortfolioAsync(int userId)
    {
        var wallet = await _context.Wallets
            .Include(w => w.Portfolio)
            .ThenInclude(pi => pi.Cryptocurrency)
            .FirstOrDefaultAsync(w => w.UserId == userId);

        if (wallet == null)
            throw new KeyNotFoundException("Wallet not found.");

        return new PortfolioResponseDto
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
    }
}