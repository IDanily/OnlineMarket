using OnlineMarket.Core.Models;

namespace OnlineMarket.Core.Abstractions
{
    public interface IOrderService : IEntityService<Order>
    {
        Task<int> GetOrderItemCountAsync(int userId);
    }
}
