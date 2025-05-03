using Crypto.DTOs;
using Crypto.Services;
using Microsoft.AspNetCore.Mvc;

namespace Crypto.Controllers;

[Route("api/trade")]
[ApiController]
public class TradesController : ControllerBase
{
    private readonly ITradeService _tradeService;

    public TradesController(ITradeService tradeService)
    {
        _tradeService = tradeService;
    }

    [HttpPost("buy")]
    public async Task<IActionResult> Buy(TradeDto dto)
    {
        try
        {
            var (message, newBalance) = await _tradeService.BuyAsync(dto);
            return Ok(new { Message = message, NewBalance = newBalance });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("sell")]
    public async Task<IActionResult> Sell(TradeDto dto)
    {
        try
        {
            var (message, newBalance) = await _tradeService.SellAsync(dto);
            return Ok(new { Message = message, NewBalance = newBalance });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}