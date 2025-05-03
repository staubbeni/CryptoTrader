using Crypto.DTOs;
using Crypto.Services;
using Microsoft.AspNetCore.Mvc;

namespace Crypto.Controllers;

[Route("api/cryptos")]
[ApiController]
public class CryptocurrenciesController : ControllerBase
{
    private readonly ICryptocurrencyService _cryptocurrencyService;

    public CryptocurrenciesController(ICryptocurrencyService cryptocurrencyService)
    {
        _cryptocurrencyService = cryptocurrencyService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCryptocurrencies()
    {
        var cryptos = await _cryptocurrencyService.GetCryptocurrenciesAsync();
        return Ok(cryptos);
    }

    [HttpGet("{cryptoId}")]
    public async Task<IActionResult> GetCrypto(int cryptoId)
    {
        try
        {
            var crypto = await _cryptocurrencyService.GetCryptoAsync(cryptoId);
            return Ok(crypto);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateCrypto(CryptoCreateDto dto)
    {
        try
        {
            var crypto = await _cryptocurrencyService.CreateCryptoAsync(dto);
            return CreatedAtAction(nameof(GetCrypto), new { cryptoId = crypto.Id }, crypto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{cryptoId}")]
    public async Task<IActionResult> DeleteCrypto(int cryptoId)
    {
        try
        {
            await _cryptocurrencyService.DeleteCryptoAsync(cryptoId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}