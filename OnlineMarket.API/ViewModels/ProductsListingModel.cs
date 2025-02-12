using OnlineMarket.Core.Models;
using System.Security.Claims;

namespace OnlineMarket.API.ViewModels
{
    public class ProductsListingModel
    {
        public List<Product> Products { get; set; }
        public ProductFilterViewModel Filter { get; set; }
        public ClaimsPrincipal User { get; set; }
        public int? Number { get; set; }
        public string Picture { get; set; }
        public string Discription { get; set; }
        public DateTime DateExpiration { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsSeller { get; set; }
        public string CategoryName { get; set; }
        public int? CategoryId { get; set; }
        public List<Category> Categories { get; set; }
    }
}
