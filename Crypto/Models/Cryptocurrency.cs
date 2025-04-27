namespace Crypto.Models
{
    public class Cryptocurrency
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public double CurrentPrice { get; set; } 
        public List<PriceHistory> PriceHistory { get; set; }
    }
}
