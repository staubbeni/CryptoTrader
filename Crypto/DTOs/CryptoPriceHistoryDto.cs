namespace Crypto.DTOs;

public class CryptoPriceHistoryDto
{
    public int CryptocurrencyId { get; set; }
    public double Price { get; set; }
    public DateTime Timestamp { get; set; }
}