namespace Crypto.DTOs;

public class PortfolioResponseDto
{
    public int WalletId { get; set; }
    public double Balance { get; set; }
    public List<PortfolioDto> PortfolioItems { get; set; }
}