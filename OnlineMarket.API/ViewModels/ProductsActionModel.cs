using OnlineMarket.Core.Models;

namespace OnlineMarket.API.ViewModels
{
    public class ProductsActionModel : EditImageModel
    {
        public int Id { get; set; }
        public string ProductId { get; set; }
        public int? Number { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateExpiration { get; set; }
        public int? UsersId { get; set; }
        public Users? Users { get; set; }
        public int CategoryId { get; set; }
        public List<Order> Orders { get; set; }
        public Category Category { get; set; }
        public string CategoryName { get; set; }
        public DateTime DateCreate { get; set; }
        public string PicturePath { get; set; }
        public int Quantity { get; set; }
        public List<CompetitorPrice> CompetitorPrices { get; set; }

        public ProductsActionModel()
        {
            DateAdded = DateTime.Now;
        }
    }
}
