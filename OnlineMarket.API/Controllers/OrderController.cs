using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineMarket.API.ViewModels;
using OnlineMarket.Application.Services;
using OnlineMarket.Core.Abstractions;
using OnlineMarket.Core.Models;
using OnlineMarket.DataBase;
using OnlineMarket.DataBase.Entites;

namespace OnlineMarket.API.Controllers
{
    public class OrderController(MarketStoreDbContext context, IOrderService orderService, TelegramService telegramService) : Controller
    {
        private readonly TelegramService _telegramService = telegramService;
        private readonly IOrderService _orderService = orderService;
        private readonly MarketStoreDbContext _context = context;

        [Authorize]
        public IActionResult GetOrder()
        {
            var userAutorize = HttpContext.User;
            var user = _context.Users.Where(_ => _.UserName == userAutorize.Identity.Name).Select(c => new Users() { Id = c.Id, UserName = c.UserName, Password = c.Password, RoleId = c.RoleId }).FirstOrDefault();
            var orderUser = _context.OrderProducts.Include(_ => _.Order).Include(_ => _.Product).Where(_ => _.Order.UsersId == user.Id && _.IsSold == false).ToList();

            return View(orderUser);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddToOrder(ProductsActionModel product)
        {
            if (product == null)
            {
                return BadRequest("Продукт не указан.");
            }

            var userAutorize = HttpContext.User;
            var user = await _context.Users.FirstOrDefaultAsync(_ => _.UserName == userAutorize.Identity.Name);

            if (user == null)
            {
                return BadRequest("Пользователь не найден.");
            }

            // Получаем существующий заказ пользователя
            var order = await _context.Orders
                .Include(o => o.OrderProduct) // Загружаем связанные продукты
                .ThenInclude(op => op.Product)
                .FirstOrDefaultAsync(o => o.UsersId == user.Id);

            // Если заказа нет, создаём новый
            if (order == null)
            {
                order = new OrderEntity
                {
                    UsersId = user.Id,
                    Users = user,
                    OrderProduct = new List<OrderProduct>()
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
            }

            // Получаем продукт из базы
            var productDb = await _context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
            if (productDb == null)
            {
                return BadRequest("Продукт не найден.");
            }

            // Проверяем, есть ли уже этот продукт в корзине
            var existingOrderProduct = order.OrderProduct.Where(_ => _.IsSold == false)
                .FirstOrDefault(op => op.ProductId == productDb.Id);

            if (existingOrderProduct != null)
            {
                // Если продукт уже есть в корзине, увеличиваем количество
                existingOrderProduct.Quantity++;
            }
            else
            {
                // Если продукта нет в корзине, добавляем новый
                var orderProduct = new OrderProduct
                {
                    ProductId = productDb.Id,
                    Product = productDb,
                    Quantity = 1
                };

                order.OrderProduct.Add(orderProduct);
            }

            await _context.SaveChangesAsync();

            return Json(new { count = order.OrderProduct.Sum(op => op.Quantity) });
        }

        public async Task<IActionResult> RemoveFromOrderAsync(int id)
        {
            var userAutorize = HttpContext.User;
            var user = await _context.Users
                .Where(_ => _.UserName == userAutorize.Identity.Name)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return BadRequest("Пользователь не найден.");
            }

            var orderDb = await _context.OrderProducts
                .Include(o => o.Order)  // Загружаем связанные продукты
                .Include(op => op.Product)
                .Where(o => o.Order.UsersId == user.Id && o.IsSold == false).ToListAsync();

            if (orderDb == null)
            {
                return NotFound("Заказ не найден.");
            }

            var orderProduct = orderDb.FirstOrDefault(op => op.ProductId == id);

            if (orderProduct != null)
            {
                // Уменьшаем количество товара
                if (orderProduct.Quantity > 1)
                {
                    orderProduct.Quantity--;
                }
                else
                {
                    // Если количество 1, удаляем товар из корзины
                    _context.OrderProducts.Remove(orderProduct);
                }

                await _context.SaveChangesAsync();  // Сохраняем изменения
            }

            return RedirectToAction("GetOrder");
        }

        public async Task<IActionResult> ClearOrderAsync()
        {
            await Clear();

            return RedirectToAction("GetOrder");
        }

        private async Task Clear()
        {
            var userAutorize = HttpContext.User;
            var user = _context.Users
                .Where(_ => _.UserName == userAutorize.Identity.Name)
                .Select(c => new Users() { Id = c.Id, UserName = c.UserName, Password = c.Password, RoleId = c.RoleId })
                .FirstOrDefault();

            var orderDb = await _context.OrderProducts
                .Include(o => o.Order)
                .Where(o => o.Order.UsersId == user.Id && o.IsSold == false).ToListAsync();

            if (orderDb.Any())
            {
                _context.OrderProducts.RemoveRange(orderDb);
                await _context.SaveChangesAsync();
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantityAsync(int productId, int quantity)
        {
            if (quantity < 1)
            {
                return RedirectToAction("Index");
            }

            var userAutorize = HttpContext.User;
            var user = _context.Users
                .Where(_ => _.UserName == userAutorize.Identity.Name)
                .Select(c => new Users() { Id = c.Id, UserName = c.UserName, Password = c.Password, RoleId = c.RoleId })
                .FirstOrDefault();

            if (user == null)
            {
                return BadRequest("Пользователь не найден.");
            }

            var orderDb = await _context.Orders
                .Include(o => o.OrderProduct)
                .ThenInclude(op => op.Product)
                .FirstOrDefaultAsync(o => o.UsersId == user.Id);

            if (orderDb == null)
            {
                return NotFound("Заказ не найден.");
            }

            // Получаем продукт из базы
            var productDb = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (productDb == null)
            {
                return BadRequest("Продукт не найден.");
            }

            // Проверяем, есть ли уже этот продукт в корзине
            var existingOrderProduct = orderDb.OrderProduct.Where(_ => _.IsSold == false)
                .FirstOrDefault(op => op.ProductId == productDb.Id);

            if (existingOrderProduct != null)
            {
                existingOrderProduct.Quantity = quantity;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("GetOrder");
        }

        [HttpGet]
        public async Task<ActionResult> SubmitOrderAsync()
        {
            var model = new OrderViewModel();
            var userAutorize = HttpContext.User;
            var user = _context.Users
                .Where(_ => _.UserName == userAutorize.Identity.Name)
                .Select(c => new Users() { Id = c.Id, UserName = c.UserName, Password = c.Password, RoleId = c.RoleId })
                .FirstOrDefault();

            if (user == null)
            {
                return BadRequest("Пользователь не найден.");
            }

            var orderDb = await _context.Orders
                .Include(o => o.OrderProduct)
                .ThenInclude(op => op.Product)
                .ThenInclude(op => op.Category)
                .FirstOrDefaultAsync(o => o.UsersId == user.Id);

            var productsDb = orderDb.OrderProduct.Where(_ => _.IsSold == false)
                .GroupBy(op => op.Product.Id)
                .Select(group => new
                {
                    Product = group.First().Product,
                    Quantity = group.Sum(p => p.Quantity),
                    Price = group.Sum(p => p.Product.Price * p.Quantity)
                })
                .ToList();

            orderDb.OrderProduct.ForEach(_ => _.IsSold = true);
            await _context.SaveChangesAsync();

            model.ProductsList = string.Join("\n", productsDb.Select(g => $"{g.Product.Name} x{g.Quantity}")); ;
            model.Email = user.Email;
            model.Name = user.Name;
            model.TotalPrice = productsDb.Select(p => p.Price).Sum();

            return View("SubmitOrder", model);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitOrderAsync(OrderViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.ProductsList))
            {
                ModelState.AddModelError("", "Список товаров пуст.");
                return RedirectToAction("MainBase", "Home");
            }

            // Формируем сообщение для отправки в Telegram
            var message = $"Новый заказ:\n" +
                          $"Телефон: {model.Phone}\n" +
                          $"Адрес доставки: {model.Address}\n" +
                          $"Товары: {model.ProductsList}\n" +
                          $"Общая сумма: {model.TotalPrice}";

            // Отправляем заказ в Telegram
            await _telegramService.SendMessageAsync(model.Name, model.Email, message);

            // Возвращаем результат с флагом успешного оформления
            TempData["OrderSuccess"] = true;

            await Clear();

            return RedirectToAction("OrderConfirmation");
        }

        public IActionResult OrderConfirmation()
        {
            return View();
        }
    }
}
