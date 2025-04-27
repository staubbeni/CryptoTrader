using Crypto.Models;
using System.ComponentModel.DataAnnotations.Schema;
namespace Crypto.Models
{
    public class PriceHistory
    {
        public int Id { get; set; }
        public int CryptocurrencyId { get; set; }
        public double Price { get; set; }
        public DateTime Timestamp { get; set; }

        [ForeignKey("CryptocurrencyId")]
        public Cryptocurrency Cryptocurrency { get; set; }
    }
}