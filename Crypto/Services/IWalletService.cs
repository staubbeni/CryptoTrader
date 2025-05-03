using Crypto.Data;
using Crypto.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Crypto.Services;

public interface IWalletService
{
    Task<ProfitResponseDto> GetProfitAsync(int userId);
    Task<PortfolioResponseDto> GetProfitDetailsAsync(int userId);
    Task<WalletDto> GetWalletAsync(int userId);
    Task DeleteWalletAsync(int userId);
    Task<(string Message, double NewBalance)> UpdateWalletBalanceAsync(int userId, WalletUpdateDto dto);
}

public class WalletService : IWalletService
{
    private readonly CryptoDbContext _context;

    public WalletService(CryptoDbContext context)
    {
        _context = context;
    }

    public async Task<ProfitResponseDto> GetProfitAsync(int userId)
    {
        var wallet = await _context.Wallets
            .Include(w => w.Portfolio)
            .ThenInclude(pi => pi.Cryptocurrency)
            .FirstOrDefaultAsync(w => w.UserId == userId);

        if (wallet == null)
            throw new KeyNotFoundException("Wallet not found.");

        double totalProfit = wallet.Portfolio.Sum(pi =>
            (pi.Cryptocurrency.CurrentPrice - pi.PurchasePrice) * pi.Quantity);

        totalProfit = Math.Round(totalProfit, 2);

        return new ProfitResponseDto
        {
            UserId = userId,
            TotalProfit = totalProfit
        };
    }

    public async Task<PortfolioResponseDto> GetProfitDetailsAsync(int userId)
    {
        var wallet = await _context.Wallets
            .Include(w => w.Portfolio)
            .ThenInclude(pi => pi.Cryptocurrency)
            .FirstOrDefaultAsync(w => w.UserId == userId);

        if (wallet == null)
            throw new KeyNotFoundException("Wallet not found.");

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

        return new PortfolioResponseDto
        {
            WalletId = wallet.Id,
            Balance = wallet.Balance,
            PortfolioItems = portfolioItems
        };
    }

    public async Task<WalletDto> GetWalletAsync(int userId)
    {
        var wallet = await _context.Wallets
            .Include(w => w.Portfolio)
            .ThenInclude(pi => pi.Cryptocurrency)
            .FirstOrDefaultAsync(w => w.UserId == userId);

        if (wallet == null)
            throw new KeyNotFoundException("Wallet not found.");

        return new WalletDto
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
    }

    public async Task DeleteWalletAsync(int userId)
    {
        var wallet = await _context.Wallets
            .Include(w => w.Portfolio)
            .FirstOrDefaultAsync(w => w.UserId == userId);

        if (wallet == null)
            throw new KeyNotFoundException("Wallet not found.");

        _context.PortfolioItems.RemoveRange(wallet.Portfolio);
        _context.Wallets.Remove(wallet);

        await _context.SaveChangesAsync();
    }

    public async Task<(string Message, double NewBalance)> UpdateWalletBalanceAsync(int userId, WalletUpdateDto dto)
    {
        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.UserId == userId);

        if (wallet == null)
            throw new KeyNotFoundException("Wallet not found.");

        double newBalance = wallet.Balance + dto.Amount;

        if (newBalance < 0)
            throw new InvalidOperationException("Insufficient balance for this operation.");

        wallet.Balance = newBalance;

        await _context.SaveChangesAsync();

        return ("Wallet balance updated successfully", wallet.Balance);
    }
}