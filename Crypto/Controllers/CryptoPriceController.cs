using Crypto.Data;
using Crypto.DTOs;
using Crypto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Crypto.Controllers;

[Route("api/crypto/price")]
[ApiController]
public class CryptoPriceController : ControllerBase
{
    private readonly CryptoDbContext _context;

    public CryptoPriceController(CryptoDbContext context)
    {
        _context = context;
    }

    [HttpPut]
    public async Task<IActionResult> UpdatePrice(CryptoPriceUpdateDto dto)
    {
        if (dto.NewPrice < 0)
            return BadRequest("Price cannot be negative.");

        var crypto = await _context.Cryptocurrencies
            .FirstOrDefaultAsync(c => c.Id == dto.CryptoId);

        if (crypto == null)
            return NotFound("Cryptocurrency not found.");

        crypto.CurrentPrice = Math.Round(dto.NewPrice, 2);

        _context.PriceHistories.Add(new PriceHistory
        {
            CryptocurrencyId = crypto.Id,
            Price = crypto.CurrentPrice,
            Timestamp = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "Price updated successfully",
            CryptocurrencyId = crypto.Id,
            NewPrice = crypto.CurrentPrice
        });
    }

    [HttpGet("history/{cryptoId}")]
    public async Task<IActionResult> GetPriceHistory(int cryptoId)
    {
        var crypto = await _context.Cryptocurrencies
            .FirstOrDefaultAsync(c => c.Id == cryptoId);

        if (crypto == null)
            return NotFound("Cryptocurrency not found.");

        var priceHistory = await _context.PriceHistories
            .Where(ph => ph.CryptocurrencyId == cryptoId)
            .OrderByDescending(ph => ph.Timestamp)
            .Select(ph => new CryptoPriceHistoryDto
            {
                CryptocurrencyId = ph.CryptocurrencyId,
                Price = ph.Price,
                Timestamp = ph.Timestamp
            })
            .ToListAsync();

        return Ok(priceHistory);
    }
}