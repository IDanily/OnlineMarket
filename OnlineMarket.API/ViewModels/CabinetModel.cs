using OnlineMarket.Core.Models;

namespace OnlineMarket.API.ViewModels
{
    public class CabinetModel
    {
        public List<Product> Products { get; set; }
        public List<UserModel> Users { get; set; }
        public UserModel UserAutorize { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsSeller { get; set; }
        public List<OrderSummary> SellerOrders { get; set; }
    }

    public class OrderSummary
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
