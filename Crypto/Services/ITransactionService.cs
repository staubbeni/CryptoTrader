using Crypto.Data;
using Crypto.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Crypto.Services;

public interface ITransactionService
{
    Task<List<TransactionDto>> GetTransactionsAsync(int userId);
    Task<TransactionDto> GetTransactionDetailsAsync(int transactionId);
}
public class TransactionService : ITransactionService
{
    private readonly CryptoDbContext _context;

    public TransactionService(CryptoDbContext context)
    {
        _context = context;
    }

    public async Task<List<TransactionDto>> GetTransactionsAsync(int userId)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
            throw new KeyNotFoundException("User not found.");

        return await _context.Transactions
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
    }

    public async Task<TransactionDto> GetTransactionDetailsAsync(int transactionId)
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
            throw new KeyNotFoundException("Transaction not found.");

        return transaction;
    }
}