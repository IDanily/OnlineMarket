using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineMarket.DataBase;

namespace OnlineMarket.API.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly MarketStoreDbContext _context;

        public NotificationsController(MarketStoreDbContext context)
        {
            _context = context;
        }

        // Получить список уведомлений для пользователя
        public async Task<IActionResult> Index()
        {
            var userAutorize = HttpContext.User;
            var userId = _context.Users.Where(_ => _.UserName == userAutorize.Identity.Name).Select(_ => _.Id).FirstOrDefault();
            var notifications = await _context.Notification
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return View(notifications);
        }

        // Пометить уведомление как прочитанное
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            try
            {
                var notification = await _context.Notification
                .FirstOrDefaultAsync(n => n.Id == notificationId);

                if (notification != null)
                {
                    notification.IsRead = true;
                    await _context.SaveChangesAsync();
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Обработка ошибки
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserNotifications()
        {
            var userAutorize = HttpContext.User;
            var userId = _context.Users.Where(_ => _.UserName == userAutorize.Identity.Name).Select(_ => _.Id).FirstOrDefault();
            var notifications = await _context.Notification
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return Json(notifications);
        }
    }
}
