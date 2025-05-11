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

    [HttpPost("limit-buy")]
    public async Task<IActionResult> LimitBuy(LimitOrderDto dto)
    {
        try
        {
            var (message, orderId) = await _tradeService.CreateLimitBuyAsync(dto);
            return Ok(new { Message = message, OrderId = orderId });
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
    [HttpPost("limit-sell")]
    public async Task<IActionResult> LimitSell(LimitOrderDto dto)
    {
        try
        {
            var (message, orderId) = await _tradeService.CreateLimitSellAsync(dto);
            return Ok(new { Message = message, OrderId = orderId });
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

    [HttpGet("limit-orders/{userId}")]
    public async Task<IActionResult> GetLimitOrders(int userId)
    {
        try
        {
            var orders = await _tradeService.GetLimitOrdersAsync(userId);
            return Ok(orders);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("limit-orders/{orderId}")]
    public async Task<IActionResult> CancelLimitOrder(int orderId)
    {
        try
        {
            var (message, orderIdResult) = await _tradeService.CancelLimitOrderAsync(orderId);
            return Ok(new { Message = message, OrderId = orderIdResult });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}