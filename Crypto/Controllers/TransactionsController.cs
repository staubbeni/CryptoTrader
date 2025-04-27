using Crypto.Data;
using Crypto.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Crypto.Controllers;

[Route("api/transactions")]
[ApiController]
public class TransactionsController : ControllerBase
{
    private readonly CryptoDbContext _context;

    public TransactionsController(CryptoDbContext context)
    {
        _context = context;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetTransactions(int userId)
    {
        var user = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!user)
            return NotFound("User not found.");

        var transactions = await _context.Transactions
            .Where(t => t.UserId == userId)
            .Include(t => t.Cryptocurrency)
            .OrderBy(t => t.Timestamp)
            .Select(t => new TransactionDto
            {
                Id = t.Id,
                UserId = t.UserId,
                CryptocurrencyId = t.CryptocurrencyId,
                CryptocurrencyName = t.Cryptocurrency.Name,
                CryptocurrencySymbol = t.Cryptocurrency.Symbol,
                Type = t.Type,
                Quantity = t.Quantity,
                Price = t.Price,
                Timestamp = t.Timestamp
            })
            .ToListAsync();

        return Ok(transactions);
    }

    [HttpGet("details/{transactionId}")]
    public async Task<IActionResult> GetTransactionDetails(int transactionId)
    {
        var transaction = await _context.Transactions
            .Include(t => t.Cryptocurrency)
            .Where(t => t.Id == transactionId)
            .Select(t => new TransactionDto
            {
                Id = t.Id,
                UserId = t.UserId,
                CryptocurrencyId = t.CryptocurrencyId,
                CryptocurrencyName = t.Cryptocurrency.Name,
                CryptocurrencySymbol = t.Cryptocurrency.Symbol,
                Type = t.Type,
                Quantity = t.Quantity,
                Price = t.Price,
                Timestamp = t.Timestamp
            })
            .FirstOrDefaultAsync();

        if (transaction == null)
            return NotFound("Transaction not found.");

        return Ok(transaction);
    }
}