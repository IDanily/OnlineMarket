namespace OnlineMarket.DataBase.Entites
{
    public class CategoryEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateExpiration { get; set; }
        public List<ProductEntity> Products { get; set; }
    }
}
