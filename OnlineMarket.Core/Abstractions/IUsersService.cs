using OnlineMarket.Core.Models;

namespace OnlineMarket.Application.Services
{
    public interface IUsersService
    {
        Task<int> CreateUser(Users user);
        void DeleteUser(int id);
        Task<List<Users>> GetAllUsers();
        void UpdateUser(int id, string userName, string name, string password, Role role, string email);
    }
}