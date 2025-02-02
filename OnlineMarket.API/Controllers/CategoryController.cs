using Microsoft.AspNetCore.Mvc;
using OnlineMarket.DataBase;

namespace OnlineMarket.API.Controllers
{
    public class CategoryController(MarketStoreDbContext context) : Controller
    {
        private readonly MarketStoreDbContext _context = context;

        // GET: Category/List
        public IActionResult List()
        {
            var categories = _context.Categories.ToList();
            return Json(categories);
        }
    }

}
