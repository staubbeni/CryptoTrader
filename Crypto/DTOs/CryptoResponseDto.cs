namespace Crypto.DTOs;

public class CryptoResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Symbol { get; set; }
    public double CurrentPrice { get; set; }
}