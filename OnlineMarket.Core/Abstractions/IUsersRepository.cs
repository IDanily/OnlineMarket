using OnlineMarket.Core.Models;

namespace OnlineMarket.Core.Abstractions
{
    public interface IUsersRepository
    {
        Task<int> Create(Users t);
        void Delete(int id);
        Task<List<Users>> Get();
        void Update(int id, string userName, string name, string password, Role role, string email);
    }
}