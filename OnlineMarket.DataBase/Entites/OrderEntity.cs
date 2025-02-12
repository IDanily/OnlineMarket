using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineMarket.DataBase.Entites
{
    public class OrderEntity
    {
        public int Id { get; set; }
        public int? Number { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? DateCreate { get; set; }
        public DateTime? DateExpiration { get; set; }
        public int? UsersId { get; set; }
        public UsersEntity? Users { get; set; }
        public List<OrderProduct> OrderProduct { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
    }
}
