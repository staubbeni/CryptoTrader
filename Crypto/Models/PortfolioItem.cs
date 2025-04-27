using System.ComponentModel.DataAnnotations.Schema;

namespace Crypto.Models
{
    public class PortfolioItem
    {
        public int Id { get; set; }
        public int WalletId { get; set; }
        public int CryptocurrencyId { get; set; }
        public double Quantity { get; set; } 
        public double PurchasePrice { get; set; } 

        [ForeignKey("WalletId")]
        public Wallet Wallet { get; set; }

        [ForeignKey("CryptocurrencyId")]
        public Cryptocurrency Cryptocurrency { get; set; }
    }
}
