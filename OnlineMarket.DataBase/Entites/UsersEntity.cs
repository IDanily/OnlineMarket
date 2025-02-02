using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace OnlineMarket.DataBase.Entites
{
    public class UsersEntity
    {
        [Required]
        [DataMember(Name = "key")]
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        [ForeignKey("Role")]
        public int? RoleId { get; set; }

        [IgnoreDataMember]
        public RoleEntity Role { get; set; }

        [IgnoreDataMember]
        public OrderEntity Order { get; set; }
        [IgnoreDataMember]
        public List<ProductEntity> Products { get; set; }
    }
}
