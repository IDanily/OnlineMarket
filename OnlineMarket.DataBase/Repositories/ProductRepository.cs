using Microsoft.EntityFrameworkCore;
using OnlineMarket.Core.Abstractions;
using OnlineMarket.Core.Models;
using OnlineMarket.DataBase.Entites;

namespace OnlineMarket.DataBase.Repositories
{
    public class ProductRepository(MarketStoreDbContext context) : IEntityRepository<Product>
    {
        private readonly MarketStoreDbContext _context = context;

        public async Task<List<Product>> GetAll()
        {
            var productsEntities = await _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .ToListAsync();

            var products = productsEntities
                .Select(e => Product.Get(e.Id, e.Number, e.Description, e.Name, e.Picture, e.Category.Name, e.Price).Products)
                .ToList();

            return products;
        }

        public async Task<int> Create(Product entity)
        {
            var prodcutEntites = new ProductEntity()
            {
                Name = entity.Name,
                Number = entity.Number,
                Description = entity.Description,
                DateCreate = DateTime.Now,
                Picture = entity.Picture,
                CategoryId = entity.Category.Id,
                Price = entity.Price,
                SellerId = entity.Seller.Id,
            };

            await _context.Products.AddAsync(prodcutEntites);
            await _context.SaveChangesAsync();

            return prodcutEntites.Id;
        }

        public async Task<int> Update(int id, int? number, string description, string name, string picture, Category category, decimal price)
        {
            var db = _context.Products
                 .FirstOrDefault(_ => _.Id == id);

            db.Name = name;
            db.Number = number;
            db.Description = description;
            db.Picture = picture;
            db.Category = _context.Categories.FirstOrDefaultAsync(_ => _.Id == category.Id).Result;
            db.Price = price;

            await _context.SaveChangesAsync();
            return db.Id;
        }

        public async Task<int> Delete(int id)
        {
            var db = _context.Products.FirstOrDefault(_ => _.Id == id);

            if (db != null)
                _context.Remove(db);

            await _context.SaveChangesAsync();
            return db.Id;
        }

        public async Task<int> Update(Product entity)
        {
            var db = _context.Products.FirstOrDefault(_ => _.Id == entity.Id);

            db.Name = entity.Name;
            db.Number = entity.Number;
            db.Description = entity.Description;
            db.Picture = entity.Picture;
            db.Category = _context.Categories.FirstOrDefaultAsync(_ => _.Id == entity.Category.Id).Result;
            db.Price = entity.Price;

            await _context.SaveChangesAsync();
            return db.Id;
        }

        public async Task<Product> Get(int id)
        {
            var productsEntities = await _context.Products.Include(_ => _.Category).AsNoTracking().ToListAsync();

            var entityDb = productsEntities.Where(_ => _.Id == id).Select(e => Product.Get(e.Id, e.Number, e.Description, e.Name, e.Picture, e.Category.Name, e.Price).Products).FirstOrDefault();

            return entityDb;
        }
    }
}
