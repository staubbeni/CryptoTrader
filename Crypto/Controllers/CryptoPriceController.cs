using Crypto.DTOs;
using Crypto.Services;
using Microsoft.AspNetCore.Mvc;

namespace Crypto.Controllers;

[Route("api/crypto/price")]
[ApiController]
public class CryptoPriceController : ControllerBase
{
    private readonly ICryptoPriceService _cryptoPriceService;

    public CryptoPriceController(ICryptoPriceService cryptoPriceService)
    {
        _cryptoPriceService = cryptoPriceService;
    }

    [HttpPut]
    public async Task<IActionResult> UpdatePrice(CryptoPriceUpdateDto dto)
    {
        try
        {
            var (message, cryptocurrencyId, newPrice) = await _cryptoPriceService.UpdatePriceAsync(dto);
            return Ok(new { Message = message, CryptocurrencyId = cryptocurrencyId, NewPrice = newPrice });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("history/{cryptoId}")]
    public async Task<IActionResult> GetPriceHistory(int cryptoId)
    {
        try
        {
            var priceHistory = await _cryptoPriceService.GetPriceHistoryAsync(cryptoId);
            return Ok(priceHistory);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}