namespace OnlineMarket.Core.Models
{
    public class CompetitorPrice
    {
        public string PlatformName { get; set; }
        public decimal Price { get; set; }
        public string Url { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
