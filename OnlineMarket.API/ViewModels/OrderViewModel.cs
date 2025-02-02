namespace OnlineMarket.API.ViewModels
{
    public class OrderViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string ProductsList { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
