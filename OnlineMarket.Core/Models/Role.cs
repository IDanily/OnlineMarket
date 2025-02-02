using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace OnlineMarket.Core.Models
{
    public class Role
    {
        [Required]
        [DataMember(Name = "key")]
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Users> Users { get; set; }
        public Role(string name)
        {
            Name = name;
            Users = new List<Users>();
        }
        public Role()
        {
            Users = new List<Users>();
        }
    }
}
