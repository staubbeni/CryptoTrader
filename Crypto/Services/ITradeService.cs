using Crypto.Data;
using Crypto.DTOs;
using Crypto.Models;
using Microsoft.EntityFrameworkCore;

namespace Crypto.Services;

public interface ITradeService
{
    Task<(string Message, double NewBalance)> BuyAsync(TradeDto dto);
    Task<(string Message, double NewBalance)> SellAsync(TradeDto dto);

    Task<(string Message, int OrderId)> CreateLimitBuyAsync(LimitOrderDto dto);
    Task<(string Message, int OrderId)> CreateLimitSellAsync(LimitOrderDto dto);
    Task<List<LimitOrderResponseDto>> GetLimitOrdersAsync(int userId);
    Task<(string Message, int OrderId)> CancelLimitOrderAsync(int orderId);
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
    public async Task<(string Message, int OrderId)> CreateLimitBuyAsync(LimitOrderDto dto)
    {
        if (dto.Amount <= 0)
            throw new ArgumentException("Quantity must be positive.");
        if (dto.LimitPrice <= 0)
            throw new ArgumentException("Limit price must be positive.");
        if (dto.ExpirationTime <= DateTime.UtcNow)
            throw new ArgumentException("Expiration time must be in the future.");

        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.UserId == dto.UserId);
        var crypto = await _context.Cryptocurrencies
            .FirstOrDefaultAsync(c => c.Id == dto.CryptoId);

        if (wallet == null)
            throw new KeyNotFoundException("Wallet not found.");
        if (crypto == null)
            throw new KeyNotFoundException("Cryptocurrency not found.");

        double estimatedCost = dto.Amount * dto.LimitPrice;
        if (wallet.Balance < estimatedCost)
            throw new InvalidOperationException("Insufficient balance for limit order.");

        var limitOrder = new LimitOrder
        {
            UserId = dto.UserId,
            CryptocurrencyId = dto.CryptoId,
            Type = "Buy",
            Amount = dto.Amount,
            LimitPrice = dto.LimitPrice,
            ExpirationTime = dto.ExpirationTime,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.LimitOrders.Add(limitOrder);
        await _context.SaveChangesAsync();

        return ("Limit buy order created successfully", limitOrder.Id);
    }
    public async Task<(string Message, int OrderId)> CreateLimitSellAsync(LimitOrderDto dto)
    {
        if (dto.Amount <= 0)
            throw new ArgumentException("Quantity must be positive.");
        if (dto.LimitPrice <= 0)
            throw new ArgumentException("Limit price must be positive.");
        if (dto.ExpirationTime <= DateTime.UtcNow)
            throw new ArgumentException("Expiration time must be in the future.");

        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.UserId == dto.UserId);
        var crypto = await _context.Cryptocurrencies
            .FirstOrDefaultAsync(c => c.Id == dto.CryptoId);

        if (wallet == null)
            throw new KeyNotFoundException("Wallet not found.");
        if (crypto == null)
            throw new KeyNotFoundException("Cryptocurrency not found.");

        var portfolioItem = await _context.PortfolioItems
            .FirstOrDefaultAsync(pi => pi.WalletId == wallet.Id && pi.CryptocurrencyId == dto.CryptoId);

        if (portfolioItem == null || portfolioItem.Quantity < dto.Amount)
            throw new InvalidOperationException("Insufficient cryptocurrency quantity for limit sell order.");

        var limitOrder = new LimitOrder
        {
            UserId = dto.UserId,
            CryptocurrencyId = dto.CryptoId,
            Type = "Sell",
            Amount = dto.Amount,
            LimitPrice = dto.LimitPrice,
            ExpirationTime = dto.ExpirationTime,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.LimitOrders.Add(limitOrder);
        await _context.SaveChangesAsync();

        return ("Limit sell order created successfully", limitOrder.Id);
    }

    public async Task<List<LimitOrderResponseDto>> GetLimitOrdersAsync(int userId)
    {
        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.UserId == userId);

        if (wallet == null)
            throw new KeyNotFoundException("Wallet not found.");

        return await _context.LimitOrders
            .Where(lo => lo.UserId == userId && lo.IsActive)
            .Join(_context.Cryptocurrencies,
                lo => lo.CryptocurrencyId,
                c => c.Id,
                (lo, c) => new LimitOrderResponseDto
                {
                    OrderId = lo.Id,
                    UserId = lo.UserId,
                    CryptocurrencyId = lo.CryptocurrencyId,
                    CryptocurrencyName = c.Name,
                    CryptocurrencySymbol = c.Symbol,
                    Type = lo.Type,
                    Amount = lo.Amount,
                    LimitPrice = lo.LimitPrice,
                    ExpirationTime = lo.ExpirationTime,
                    IsActive = lo.IsActive,
                    CreatedAt = lo.CreatedAt
                })
            .ToListAsync();
    }

    public async Task<(string Message, int OrderId)> CancelLimitOrderAsync(int orderId)
    {
        var limitOrder = await _context.LimitOrders
            .FirstOrDefaultAsync(lo => lo.Id == orderId && lo.IsActive);

        if (limitOrder == null)
            throw new KeyNotFoundException("Limit order not found or already inactive.");

        limitOrder.IsActive = false;
        await _context.SaveChangesAsync();

        return ("Limit order cancelled successfully", limitOrder.Id);
    }
}