using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineMarket.Core.Abstractions;
using OnlineMarket.DataBase;

namespace OnlineMarket.API.ViewComponents
{
    public class OrderViewComponent(MarketStoreDbContext context, IOrderService orderService) : ViewComponent
    {
        private readonly IOrderService _orderService = orderService;
        private readonly MarketStoreDbContext _context = context;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userAutorize = HttpContext.User;
            var user = await _context.Users.FirstOrDefaultAsync(_ => _.UserName == userAutorize.Identity.Name);
            int orderItemCount = 0;

            var orderDb = await _context.Orders
                .Include(o => o.OrderProduct)
                .ThenInclude(op => op.Product)
                .FirstOrDefaultAsync(o => o.UsersId == user.Id);

            if (user != null && orderDb != null && orderDb.OrderProduct.Where(_ => _.IsSold == false).ToList().Any())
                foreach (var product in orderDb.OrderProduct.Where(_ => _.IsSold == false))
                    orderItemCount += product.Quantity;

            return View(orderItemCount);
        }
    }
}
