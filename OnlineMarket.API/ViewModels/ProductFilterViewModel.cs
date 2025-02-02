using OnlineMarket.Core.Models;

namespace OnlineMarket.API.ViewModels
{
    public class ProductFilterViewModel
    {
        public int? CategoryId { get; set; }
        public Category Category { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string Query { get; set; }
    }

}
