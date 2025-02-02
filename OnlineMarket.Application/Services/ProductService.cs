using OnlineMarket.Core.Abstractions;
using OnlineMarket.Core.Models;

namespace OnlineMarket.Application.Services
{
    public class ProductService : IEntityService<Product>
    {
        private readonly IEntityRepository<Product> _repository;

        public ProductService(IEntityRepository<Product> entityRepository)
        {
            _repository = entityRepository;
        }

        public async Task<int> Create(Product entity)
        {
            return await _repository.Create(entity);
        }

        public async Task<int> Delete(int id)
        {
            return await _repository.Delete(id);
        }

        public async Task<int> Update(Product entity)
        {
            return await _repository.Update(entity);
        }

        public async Task<List<Product>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task<Product> Get(int id)
        {
            return await _repository.Get(id);
        }
    }
}
