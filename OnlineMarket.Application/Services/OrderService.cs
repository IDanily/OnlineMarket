using OnlineMarket.Core.Abstractions;
using OnlineMarket.Core.Models;

namespace OnlineMarket.Application.Services
{
    public class OrderService(IOrderRepository entityRepository) : IOrderService
    {
        private readonly IOrderRepository _repository = entityRepository;

        public async Task<int> Create(Order entity)
        {
            return await _repository.Create(entity);
        }

        async Task<List<Order>> IEntityService<Order>.GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task<Order> Get(int id)
        {
            return await _repository.Get(id);
        }

        public async Task<int> Delete(int id)
        {
            return await _repository.Delete(id);
        }

        public async Task<int> Update(Order entity)
        {
            return await _repository.Update(entity);
        }

        public async Task<int> GetOrderItemCountAsync(int userId)
        {
            return await _repository.GetOrderItemCountAsync(userId);
        }
    }
}
