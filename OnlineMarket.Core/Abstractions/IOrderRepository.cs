using OnlineMarket.Core.Models;

namespace OnlineMarket.Core.Abstractions
{
    public interface IOrderRepository : IEntityRepository<Order>
    {
        Task<int> GetOrderItemCountAsync(int userId);
    }
}
