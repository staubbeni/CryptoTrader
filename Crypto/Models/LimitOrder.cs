namespace Crypto.Models;
public class LimitOrder
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CryptocurrencyId { get; set; }
    public string Type { get; set; }
    public double Amount { get; set; }
    public double LimitPrice { get; set; }
    public DateTime ExpirationTime { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}