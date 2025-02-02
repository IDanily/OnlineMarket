namespace OnlineMarket.Core.Models
{
    public class Product
    {
        public int Id { get; set; }
        public int? Number { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateExpiration { get; set; }
        public List<Order> Orders { get; set; }
        public Category Category { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public string Picture { get; set; }
        public Users Seller { get; set; }
        public List<CompetitorPrice> CompetitorPrices { get; set; }
        public static (Product Products, string Error) Create(int id, int? number, string description, string name, string picture, decimal price, Category category, Users seller)
        {
            var error = string.Empty;

            if (string.IsNullOrEmpty(description))
            {
                error = "Необходимо заполнить описание";
            }

            var product = new Product()
            {
                Id = id,
                Number = number,
                Name = name,
                Description = description,
                DateCreate = DateTime.Now,
                Picture = picture,
                Price = price,
                Category = category,
                Seller = seller
            };

            return (product, error);
        }

        public static (Product Products, string Error) Get(int id, int? number, string description, string name, string picture, string categoryName, decimal price)
        {
            var error = string.Empty;

            if (string.IsNullOrEmpty(description))
            {
                error = "Необходимо заполнить описание";
            }

            var product = new Product()
            {
                Id = id,
                Number = number,
                Name = name,
                Description = description,
                DateCreate = DateTime.Now,
                Picture = picture,
                CategoryName = categoryName,
                Price = price
            };

            return (product, error);
        }
    }
}
