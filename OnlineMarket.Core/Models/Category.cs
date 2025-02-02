namespace OnlineMarket.Core.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateExpiration { get; set; }

        public static (Category Categores, string Error) Get(int id, string description, string name, string code)
        {
            var error = string.Empty;

            var category = new Category()
            {
                Id = id,
                Name = name,
                Description = description,
                DateCreate = DateTime.Now,
                Code = code
            };

            return (category, error);
        }
    }
}
