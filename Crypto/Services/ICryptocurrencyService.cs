using Crypto.Data;
using Crypto.DTOs;
using Crypto.Models;
using Microsoft.EntityFrameworkCore;

namespace Crypto.Services;
public interface ICryptocurrencyService
{
    Task<List<object>> GetCryptocurrenciesAsync();
    Task<CryptoResponseDto> GetCryptoAsync(int cryptoId);
    Task<CryptoResponseDto> CreateCryptoAsync(CryptoCreateDto dto);
    Task DeleteCryptoAsync(int cryptoId);
}

public class CryptocurrencyService : ICryptocurrencyService
{
    private readonly CryptoDbContext _context;

    public CryptocurrencyService(CryptoDbContext context)
    {
        _context = context;
    }

    public async Task<List<object>> GetCryptocurrenciesAsync()
    {
        return await _context.Cryptocurrencies
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Symbol,
                c.CurrentPrice
            })
            .ToListAsync<object>();
    }

    public async Task<CryptoResponseDto> GetCryptoAsync(int cryptoId)
    {
        var crypto = await _context.Cryptocurrencies
            .FirstOrDefaultAsync(c => c.Id == cryptoId);

        if (crypto == null)
            throw new KeyNotFoundException("Cryptocurrency not found.");

        return new CryptoResponseDto
        {
            Id = crypto.Id,
            Name = crypto.Name,
            Symbol = crypto.Symbol,
            CurrentPrice = crypto.CurrentPrice
        };
    }

    public async Task<CryptoResponseDto> CreateCryptoAsync(CryptoCreateDto dto)
    {
        if (await _context.Cryptocurrencies.AnyAsync(c => c.Symbol == dto.Symbol))
            throw new InvalidOperationException("Cryptocurrency symbol already exists.");

        if (dto.CurrentPrice < 0)
            throw new ArgumentException("Current price cannot be negative.");

        var crypto = new Cryptocurrency
        {
            Name = dto.Name,
            Symbol = dto.Symbol,
            CurrentPrice = dto.CurrentPrice
        };

        _context.Cryptocurrencies.Add(crypto);
        await _context.SaveChangesAsync();

        return new CryptoResponseDto
        {
            Id = crypto.Id,
            Name = crypto.Name,
            Symbol = crypto.Symbol,
            CurrentPrice = crypto.CurrentPrice
        };
    }

    public async Task DeleteCryptoAsync(int cryptoId)
    {
        var crypto = await _context.Cryptocurrencies
            .FirstOrDefaultAsync(c => c.Id == cryptoId);

        if (crypto == null)
            throw new KeyNotFoundException("Cryptocurrency not found.");

        var hasPortfolioItems = await _context.PortfolioItems.AnyAsync(pi => pi.CryptocurrencyId == cryptoId);
        var hasTransactions = await _context.Transactions.AnyAsync(t => t.CryptocurrencyId == cryptoId);

        if (hasPortfolioItems || hasTransactions)
            throw new InvalidOperationException("Cannot delete cryptocurrency with existing portfolio items or transactions.");

        var priceHistories = await _context.PriceHistories
            .Where(ph => ph.CryptocurrencyId == cryptoId)
            .ToListAsync();
        _context.PriceHistories.RemoveRange(priceHistories);

        _context.Cryptocurrencies.Remove(crypto);
        await _context.SaveChangesAsync();
    }
}