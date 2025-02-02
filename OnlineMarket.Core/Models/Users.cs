using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace OnlineMarket.Core.Models
{
    public class Users
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
        public Role? Role { get; set; }
        public static (Users Users, string Error) Create( string userName, string name, string password, int roleId, string email)
        {
            var error = string.Empty;

            if (string.IsNullOrEmpty(userName))
            {
                error = "Необходимо заполнить имя пользователя";
            }

            var users = new Users()
            {
                UserName = userName,
                Name = name,
                Password = password,
                RoleId = roleId,
                Email = email
            };

            return (users, error);
        }
    }
}
