namespace OnlineMarket.Core.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int? Number { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreate { get; set; } = DateTime.UtcNow;
        public DateTime DateExpiration { get; set; } = DateTime.UtcNow;
        public int? UsersId { get; set; }
        public Users? Users { get; set; }
        public decimal Price { get; set; }
        public List<Product> Products { get; set; }
        public Order()
        {
            Products = new List<Product>();
        }

        public static (Order Orders, string Error) Create(Order entity)
        {
            var error = string.Empty;

            if (string.IsNullOrEmpty(entity.Description))
            {
                error = "Необходимо заполнить описание";
            }

            var order = new Order()
            {
                Id = entity.Id,
                Number = entity.Number,
                Name = entity.Name,
                Description = entity.Description,
                DateCreate = DateTime.Now,
            };

            return (order, error);
        }

        public static (Order Orders, string Error) Get(int id, int? number, string description, string name)
        {
            var error = string.Empty;

            if (string.IsNullOrEmpty(description))
            {
                error = "Необходимо заполнить описание";
            }

            var order = new Order()
            {
                Id = id,
                Number = number,
                Name = name,
                Description = description,
                DateCreate = DateTime.Now,
            };

            return (order, error);
        }
    }
}
