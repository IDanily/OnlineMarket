namespace OnlineMarket.DataBase.Entites
{
    public class OrderProduct
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public OrderEntity Order { get; set; }

        public int ProductId { get; set; }
        public ProductEntity Product { get; set; }

        public int Quantity { get; set; }
        public bool IsSold { get; set; }
    }
}
