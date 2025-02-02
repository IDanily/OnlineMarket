using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineMarket.API.ViewModels;
using OnlineMarket.Application.Services;
using OnlineMarket.Core.Abstractions;
using OnlineMarket.Core.Models;
using OnlineMarket.DataBase;

namespace OnlineMarket.API.Controllers
{
    [AllowAnonymous]
    public class HomeController(MarketStoreDbContext context, IEntityService<Product> productService, TelegramService telegramService) : Controller
    {
        private readonly MarketStoreDbContext _context = context;
        private readonly IEntityService<Product> _productService = productService;
        private readonly TelegramService _telegramService = telegramService;

        public IActionResult MainBase(ProductFilterViewModel filter)
        {
            var products = _context.Products.Include(_ => _.Category).ToList();

            if (filter.CategoryId.HasValue)
                products = products.Where(p => p.Category.Id == filter.CategoryId).ToList();

            if (filter.MinPrice.HasValue)
                products = products.Where(p => p.Price >= filter.MinPrice.Value).ToList();

            if (filter.MaxPrice.HasValue)
                products = products.Where(p => p.Price <= filter.MaxPrice.Value).ToList();

            if (!string.IsNullOrEmpty(filter.Query))
            {
                products = products.Where(p => p.Name.Contains(filter.Query) || p.Category.Name.Contains(filter.Query) || p.Description.Contains(filter.Query)).ToList();
            }

            var productList = products.Select(_ => Product.Get(_.Id, _.Number, _.Description, _.Name, _.Picture, _.Category.Name, _.Price).Products).ToList();

            var model = new ProductsListingModel
            {
                User = HttpContext.User,
                Products = productList,
                IsAdmin = HttpContext.User.IsInRole("admin"),
                Filter = filter
            };

            ViewData["SearchQuery"] = filter.Query;

            return View(model);
        }

        [HttpGet]
        public IActionResult Search(string query)
        {
            return RedirectToAction("MainBase", new ProductsListingModel() { Filter = new ProductFilterViewModel() { Query = query } });
        }

        public IActionResult News()
        {
            return View();
        }

        public IActionResult Contacts()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string name, string email, string message)
        {
            await _telegramService.SendMessageAsync(name, email, message);
            return RedirectToAction("MainBase"); // Переход на страницу благодарности
        }
    }
}
