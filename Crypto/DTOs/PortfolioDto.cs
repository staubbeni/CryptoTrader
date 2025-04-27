
namespace Crypto.DTOs;

public class PortfolioDto
{
    public int CryptocurrencyId { get; set; }
    public string CryptocurrencyName { get; set; }
    public string CryptocurrencySymbol { get; set; }
    public double Quantity { get; set; }
    public double PurchasePrice { get; set; }
    public double CurrentPrice { get; set; }
    public double CurrentValue { get; set; }
    public double Profit { get; set; }
}