using Crypto.Data;
using Crypto.DTOs;
using Crypto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Crypto.Controllers;

[Route("api/cryptos")]
[ApiController]
public class CryptocurrenciesController : ControllerBase
{
    private readonly CryptoDbContext _context;

    public CryptocurrenciesController(CryptoDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetCryptocurrencies()
    {
        var cryptos = await _context.Cryptocurrencies
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Symbol,
                c.CurrentPrice
            })
            .ToListAsync();

        return Ok(cryptos);
    }
    [HttpGet("{cryptoId}")]
    public async Task<IActionResult> GetCrypto(int cryptoId)
    {
        var crypto = await _context.Cryptocurrencies
            .FirstOrDefaultAsync(c => c.Id == cryptoId);

        if (crypto == null)
            return NotFound("Cryptocurrency not found.");

        return Ok(new CryptoResponseDto
        {
            Id = crypto.Id,
            Name = crypto.Name,
            Symbol = crypto.Symbol,
            CurrentPrice = crypto.CurrentPrice
        });
    }

    [HttpPost]
    public async Task<IActionResult> CreateCrypto(CryptoCreateDto dto)
    {
        if (await _context.Cryptocurrencies.AnyAsync(c => c.Symbol == dto.Symbol))
            return BadRequest("Cryptocurrency symbol already exists.");

        if (dto.CurrentPrice < 0)
            return BadRequest("Current price cannot be negative.");

        var crypto = new Cryptocurrency
        {
            Name = dto.Name,
            Symbol = dto.Symbol,
            CurrentPrice = dto.CurrentPrice
        };

        _context.Cryptocurrencies.Add(crypto);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCrypto), new { cryptoId = crypto.Id }, new CryptoResponseDto
        {
            Id = crypto.Id,
            Name = crypto.Name,
            Symbol = crypto.Symbol,
            CurrentPrice = crypto.CurrentPrice
        });
    }


    [HttpDelete("{cryptoId}")]
    public async Task<IActionResult> DeleteCrypto(int cryptoId)
    {
        var crypto = await _context.Cryptocurrencies
            .FirstOrDefaultAsync(c => c.Id == cryptoId);

        if (crypto == null)
            return NotFound("Cryptocurrency not found.");

        var hasPortfolioItems = await _context.PortfolioItems.AnyAsync(pi => pi.CryptocurrencyId == cryptoId);
        var hasTransactions = await _context.Transactions.AnyAsync(t => t.CryptocurrencyId == cryptoId);

        if (hasPortfolioItems || hasTransactions)
            return BadRequest("Cannot delete cryptocurrency with existing portfolio items or transactions.");

        var priceHistories = await _context.PriceHistories
            .Where(ph => ph.CryptocurrencyId == cryptoId)
            .ToListAsync();
        _context.PriceHistories.RemoveRange(priceHistories);

        _context.Cryptocurrencies.Remove(crypto);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    

}