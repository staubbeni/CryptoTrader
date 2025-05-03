using Crypto.Data;
using Crypto.DTOs;
using Crypto.Models;
using Microsoft.EntityFrameworkCore;

namespace Crypto.Services;

public interface ITradeService
{
    Task<(string Message, double NewBalance)> BuyAsync(TradeDto dto);
    Task<(string Message, double NewBalance)> SellAsync(TradeDto dto);
}

public class TradeService : ITradeService
{
    private readonly CryptoDbContext _context;

    public TradeService(CryptoDbContext context)
    {
        _context = context;
    }

    public async Task<(string Message, double NewBalance)> BuyAsync(TradeDto dto)
    {
        if (dto.Quantity <= 0)
            throw new ArgumentException("Quantity must be positive.");

        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.UserId == dto.UserId);
        var crypto = await _context.Cryptocurrencies
            .FirstOrDefaultAsync(c => c.Id == dto.CryptocurrencyId);

        if (wallet == null)
            throw new KeyNotFoundException("Wallet not found.");
        if (crypto == null)
            throw new KeyNotFoundException("Cryptocurrency not found.");

        double cost = dto.Quantity * crypto.CurrentPrice;

        if (wallet.Balance < cost)
            throw new InvalidOperationException("Insufficient balance.");

        wallet.Balance -= cost;

        var portfolioItem = await _context.PortfolioItems
            .FirstOrDefaultAsync(pi => pi.WalletId == wallet.Id && pi.CryptocurrencyId == dto.CryptocurrencyId);

        if (portfolioItem == null)
        {
            portfolioItem = new PortfolioItem
            {
                WalletId = wallet.Id,
                CryptocurrencyId = dto.CryptocurrencyId,
                Quantity = dto.Quantity,
                PurchasePrice = crypto.CurrentPrice
            };
            _context.PortfolioItems.Add(portfolioItem);
        }
        else
        {
            double totalQuantity = portfolioItem.Quantity + dto.Quantity;
            portfolioItem.PurchasePrice =
                (portfolioItem.PurchasePrice * portfolioItem.Quantity + crypto.CurrentPrice * dto.Quantity) / totalQuantity;
            portfolioItem.Quantity = totalQuantity;
        }

        var transaction = new Transaction
        {
            UserId = dto.UserId,
            CryptocurrencyId = dto.CryptocurrencyId,
            Type = "Buy",
            Quantity = dto.Quantity,
            Price = crypto.CurrentPrice,
            Timestamp = DateTime.UtcNow
        };
        _context.Transactions.Add(transaction);

        await _context.SaveChangesAsync();
        return ("Purchase successful", wallet.Balance);
    }

    public async Task<(string Message, double NewBalance)> SellAsync(TradeDto dto)
    {
        if (dto.Quantity <= 0)
            throw new ArgumentException("Quantity must be positive.");

        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.UserId == dto.UserId);
        var crypto = await _context.Cryptocurrencies
            .FirstOrDefaultAsync(c => c.Id == dto.CryptocurrencyId);

        if (wallet == null)
            throw new KeyNotFoundException("Wallet not found.");
        if (crypto == null)
            throw new KeyNotFoundException("Cryptocurrency not found.");

        var portfolioItem = await _context.PortfolioItems
            .FirstOrDefaultAsync(pi => pi.WalletId == wallet.Id && pi.CryptocurrencyId == dto.CryptocurrencyId);

        if (portfolioItem == null || portfolioItem.Quantity < dto.Quantity)
            throw new InvalidOperationException("Insufficient cryptocurrency quantity.");

        double revenue = dto.Quantity * crypto.CurrentPrice;
        wallet.Balance += revenue;

        portfolioItem.Quantity -= dto.Quantity;
        if (portfolioItem.Quantity == 0)
            _context.PortfolioItems.Remove(portfolioItem);

        var transaction = new Transaction
        {
            UserId = dto.UserId,
            CryptocurrencyId = dto.CryptocurrencyId,
            Type = "Sell",
            Quantity = dto.Quantity,
            Price = crypto.CurrentPrice,
            Timestamp = DateTime.UtcNow
        };
        _context.Transactions.Add(transaction);

        await _context.SaveChangesAsync();
        return ("Sale successful", wallet.Balance);
    }
}