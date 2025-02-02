using OnlineMarket.Core.Abstractions;
using OnlineMarket.Core.Models;

namespace OnlineMarket.Application.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _repository;

        public UsersService(IUsersRepository usersRepository)
        {
            _repository = usersRepository;
        }

        public Task<List<Users>> GetAllUsers()
        {
            return _repository.Get();
        }

        public Task<int> CreateUser(Users user)
        {
            return _repository.Create(user);
        }

        public void UpdateUser(int id, string userName, string name, string password, Role role, string email)
        {
            _repository.Update(id, userName, name, password, role, email);
        }

        public void DeleteUser(int id)
        {
            _repository.Delete(id);
        }
    }
}
