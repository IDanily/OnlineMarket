

using Microsoft.AspNetCore.Mvc.Rendering;

namespace OnlineMarket.API.ViewModels
{
    public class UserModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string Email { get; set; }
        public List<SelectListItem> Roles { get; set; }
    }
}
