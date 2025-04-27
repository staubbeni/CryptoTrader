using Crypto.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Crypto.Models;

public class Transaction
{
    public int Id { get; set; } 
    public int UserId { get; set; }
    public int CryptocurrencyId { get; set; }
    public string Type { get; set; } 
    public double Quantity { get; set; } 
    public double Price { get; set; } 
    public DateTime Timestamp { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }

    [ForeignKey("CryptocurrencyId")]
    public Cryptocurrency Cryptocurrency { get; set; }
}