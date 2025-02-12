using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineMarket.DataBase;

namespace OnlineMarket.API.ViewComponents
{
    public class NotificationCountViewComponent : ViewComponent
    {
        private readonly MarketStoreDbContext _context;

        public NotificationCountViewComponent(MarketStoreDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userAutorize = HttpContext.User;
            var userId = await _context.Users.Where(_ => _.UserName == userAutorize.Identity.Name).Select(_ => _.Id).FirstOrDefaultAsync();
            var notificationCount = await _context.Notification
                .Where(n => n.UserId == userId && !n.IsRead)
                .CountAsync();

            return View(notificationCount);
        }
    }

}
