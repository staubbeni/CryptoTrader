namespace Crypto.DTOs;

public class LimitOrderDto
{
    public int UserId { get; set; }
    public int CryptoId { get; set; }
    public double Amount { get; set; }
    public double LimitPrice { get; set; }
    public DateTime ExpirationTime { get; set; }
}