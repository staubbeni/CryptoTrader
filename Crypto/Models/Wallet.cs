namespace Crypto.Models
{
    public class Wallet
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public double Balance { get; set; } 
        public User User { get; set; }
        public List<PortfolioItem> Portfolio { get; set; }
    }
}
