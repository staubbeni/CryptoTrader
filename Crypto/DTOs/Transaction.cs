namespace Crypto.DTOs;

public class TransactionDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CryptocurrencyId { get; set; }
    public string CryptocurrencyName { get; set; }
    public string CryptocurrencySymbol { get; set; }
    public string Type { get; set; } 
    public double Quantity { get; set; }
    public double Price { get; set; }
    public DateTime Timestamp { get; set; }
}