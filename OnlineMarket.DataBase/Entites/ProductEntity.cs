using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineMarket.DataBase.Entites
{
    public class ProductEntity
    {
        public int Id { get; set; }
        public int? Number { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateExpiration { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public List<OrderProduct> OrderProduct { get; set; }
        [Required]
        [Display(Name = "Image")]
        public string Picture { get; set; }
        public int CategoryId { get; set; }
        public CategoryEntity Category { get; set; }
        public int? SellerId { get; set; }
        public UsersEntity? Seller { get; set; }
    }
}
