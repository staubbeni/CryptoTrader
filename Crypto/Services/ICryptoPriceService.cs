using Crypto.Data;
using Crypto.DTOs;
using Crypto.Models;
using Microsoft.EntityFrameworkCore;

namespace Crypto.Services;

public interface ICryptoPriceService
{
    Task<(string Message, int CryptocurrencyId, double NewPrice)> UpdatePriceAsync(CryptoPriceUpdateDto dto);
    Task<List<CryptoPriceHistoryDto>> GetPriceHistoryAsync(int cryptoId);
}

public class CryptoPriceService : ICryptoPriceService
{
    private readonly CryptoDbContext _context;

    public CryptoPriceService(CryptoDbContext context)
    {
        _context = context;
    }

    public async Task<(string Message, int CryptocurrencyId, double NewPrice)> UpdatePriceAsync(CryptoPriceUpdateDto dto)
    {
        if (dto.NewPrice < 0)
            throw new ArgumentException("Price cannot be negative.");

        var crypto = await _context.Cryptocurrencies
            .FirstOrDefaultAsync(c => c.Id == dto.CryptoId);

        if (crypto == null)
            throw new KeyNotFoundException("Cryptocurrency not found.");

        crypto.CurrentPrice = Math.Round(dto.NewPrice, 2);

        _context.PriceHistories.Add(new PriceHistory
        {
            CryptocurrencyId = crypto.Id,
            Price = crypto.CurrentPrice,
            Timestamp = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        return ("Price updated successfully", crypto.Id, crypto.CurrentPrice);
    }

    public async Task<List<CryptoPriceHistoryDto>> GetPriceHistoryAsync(int cryptoId)
    {
        var crypto = await _context.Cryptocurrencies
            .FirstOrDefaultAsync(c => c.Id == cryptoId);

        if (crypto == null)
            throw new KeyNotFoundException("Cryptocurrency not found.");

        return await _context.PriceHistories
            .Where(ph => ph.CryptocurrencyId == cryptoId)
            .OrderByDescending(ph => ph.Timestamp)
            .Select(ph => new CryptoPriceHistoryDto
            {
                CryptocurrencyId = ph.CryptocurrencyId,
                Price = ph.Price,
                Timestamp = ph.Timestamp
            })
            .ToListAsync();
    }
}