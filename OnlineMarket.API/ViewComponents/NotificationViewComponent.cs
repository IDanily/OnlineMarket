using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineMarket.DataBase;

namespace OnlineMarket.API.ViewComponents
{
    public class NotificationViewComponent : ViewComponent
    {
        private readonly MarketStoreDbContext _context;

        public NotificationViewComponent(MarketStoreDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userAutorize = HttpContext.User;
            var userId = await _context.Users.Where(_ => _.UserName == userAutorize.Identity.Name).Select(_ => _.Id).FirstOrDefaultAsync();
            var notifications = await _context.Notification
                .Where(n => n.UserId == userId && !n.IsRead).ToListAsync();
            return View(notifications);
        }
    }
}
