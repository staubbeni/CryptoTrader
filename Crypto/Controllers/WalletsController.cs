using Crypto.DTOs;
using Crypto.Services;
using Microsoft.AspNetCore.Mvc;

namespace Crypto.Controllers;

[Route("api/wallet")]
[ApiController]
public class WalletsController : ControllerBase
{
    private readonly IWalletService _walletService;

    public WalletsController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetWallet(int userId)
    {
        try
        {
            var wallet = await _walletService.GetWalletAsync(userId);
            return Ok(wallet);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteWallet(int userId)
    {
        try
        {
            await _walletService.DeleteWalletAsync(userId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateWalletBalance(int userId, WalletUpdateDto dto)
    {
        try
        {
            var (message, newBalance) = await _walletService.UpdateWalletBalanceAsync(userId, dto);
            return Ok(new { Message = message, NewBalance = newBalance });
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