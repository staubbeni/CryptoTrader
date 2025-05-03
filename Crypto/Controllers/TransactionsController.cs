using Crypto.DTOs;
using Crypto.Services;
using Microsoft.AspNetCore.Mvc;

namespace Crypto.Controllers;

[Route("api/transactions")]
[ApiController]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetTransactions(int userId)
    {
        try
        {
            var transactions = await _transactionService.GetTransactionsAsync(userId);
            return Ok(transactions);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("details/{transactionId}")]
    public async Task<IActionResult> GetTransactionDetails(int transactionId)
    {
        try
        {
            var transaction = await _transactionService.GetTransactionDetailsAsync(transactionId);
            return Ok(transaction);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}