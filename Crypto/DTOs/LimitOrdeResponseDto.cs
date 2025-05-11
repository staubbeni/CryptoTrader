namespace Crypto.DTOs;

public class LimitOrderResponseDto
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public int CryptocurrencyId { get; set; }
    public string CryptocurrencyName { get; set; }
    public string CryptocurrencySymbol { get; set; }
    public string Type { get; set; }
    public double Amount { get; set; }
    public double LimitPrice { get; set; }
    public DateTime ExpirationTime { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}