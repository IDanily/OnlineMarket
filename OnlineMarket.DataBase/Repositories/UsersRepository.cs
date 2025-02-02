using Microsoft.EntityFrameworkCore;
using OnlineMarket.Core.Abstractions;
using OnlineMarket.Core.Models;
using OnlineMarket.DataBase;
using OnlineMarket.DataBase.Entites;

namespace OnlineMarket.DataBase.Reposotories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly MarketStoreDbContext _context;

        public UsersRepository(MarketStoreDbContext context)
        {
            _context = context;
        }

        public async Task<List<Users>> Get()
        {
            var newsEntities = await _context.Users
                .Include(_ => _.Role)
                .AsNoTracking()
                .ToListAsync();

            var news = newsEntities
                .Select(e => new Users()
                {
                    Id = e.Id,
                    UserName = e.UserName,
                    Name = e.Name,
                    Password = e.Password,
                    RoleId = e.RoleId,
                    Email = e.Email
                })
                .ToList();

            return news;
        }

        public async Task<int> Create(Users t)
        {
            var userEntites = new UsersEntity() { Id = t.Id, UserName = t.UserName, Name = t.Name, Password = t.Password, RoleId = t.RoleId, Email = t.Email };

            await _context.Users.AddAsync(userEntites);
            await _context.SaveChangesAsync();

            return userEntites.Id;
        }

        public void Update(int id, string userName, string name, string password, Role role, string email)
        {
            var db = _context.Users
                 .FirstOrDefault(_ => _.Id == id);

            db.UserName = userName;
            db.Name = name;
            db.Password = password;
            db.Email = email;

            if (role != null)
                db.RoleId = role.Id;

            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var db = _context.Users.FirstOrDefault(_ => _.Id == id);

            if (db != null)
                _context.Remove<UsersEntity>(db);

            _context.SaveChanges();
        }
    }
}
