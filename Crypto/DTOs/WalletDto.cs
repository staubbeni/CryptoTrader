namespace Crypto.DTOs;

public class WalletDto
{
    public int WalletId { get; set; }
    public double Balance { get; set; }
    public List<PortfolioItemDto> Portfolio { get; set; }
}

public class PortfolioItemDto
{
    public int CryptocurrencyId { get; set; }
    public string CryptocurrencyName { get; set; }
    public string CryptocurrencySymbol { get; set; }
    public double Quantity { get; set; }
    public double PurchasePrice { get; set; }
    public double CurrentPrice { get; set; }
    public double CurrentValue { get; set; } 
}