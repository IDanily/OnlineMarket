using Microsoft.EntityFrameworkCore;
using OnlineMarket.Core.Abstractions;
using OnlineMarket.Core.Models;
using OnlineMarket.DataBase.Entites;

namespace OnlineMarket.DataBase.Repositories
{
    public class OrderRepository(MarketStoreDbContext context) : IOrderRepository
    {
        private readonly MarketStoreDbContext _context = context;

        public async Task<int> Create(Order entity)
        {
            var orderEntites = new OrderEntity()
            {
                Name = entity.Name,
                Number = entity.Number,
                Description = entity.Description,
                DateCreate = DateTime.Now,
                UsersId = entity.Users.Id,
                Price = entity.Price
            };

            await _context.Orders.AddAsync(orderEntites);
            await _context.SaveChangesAsync();

            return orderEntites.Id;
        }

        public async Task<int> Delete(int id)
        {
            var db = _context.Orders.FirstOrDefault(_ => _.Id == id);

            if (db != null)
                _context.Remove(db);

            await _context.SaveChangesAsync();

            return db.Id;
        }

        public async Task<List<Order>> GetAll()
        {
            var oderEntities = await _context.Orders.AsNoTracking().ToListAsync();

            var news = oderEntities
                .Select(e => Order.Get(e.Id, e.Number, e.Description, e.Name).Orders)
                .ToList();

            return news;
        }

        public async Task<Order> Get(int id)
        {
            var ordersEntities = await _context.Orders.AsNoTracking().ToListAsync();

            var entityDb = ordersEntities.Where(_ => _.Id == id).Select(e => Order.Get(e.Id, e.Number, e.Description, e.Name).Orders).FirstOrDefault();

            return entityDb;
        }

        public async Task<int> Update(Order entity)
        {
            var db = _context.Orders.FirstOrDefault(_ => _.Id == entity.Id);

            if (entity.Name != null)
                db.Name = entity.Name;

            if (entity.Number != null)
                db.Number = entity.Number;

            if (entity.Description != null)
                db.Description = entity.Description;

            if (entity.Price != null)
                db.Price = entity.Price;

            await _context.SaveChangesAsync();
            return db.Id;
        }

        public async Task<int> GetOrderItemCountAsync(int userId)
        {
            if (userId == null)
                return 0;

            return await _context.Orders
                .Include(_ => _.OrderProduct)
                .ThenInclude(op => op.Product)
                .Where(o => o.UsersId == userId)
                .CountAsync();
        }
    }
}
