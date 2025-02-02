using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OfficeOpenXml;
using OnlineMarket.API.ViewModels;
using OnlineMarket.Application.Services;
using OnlineMarket.Core.Abstractions;
using OnlineMarket.Core.Models;
using OnlineMarket.DataBase;

namespace OnlineMarket.API.Controllers
{
    [Authorize]
    public class UsersController(MarketStoreDbContext context, IOrderService orderService, IUsersService service) : Controller
    {
        private readonly MarketStoreDbContext _context = context;
        private readonly IOrderService _orderService = orderService;
        private readonly IUsersService _usersService = service;

        // GET: UsersController
        public ActionResult Cabinet()
        {
            var users = _usersService.GetAllUsers().Result.Select(c => new UserModel() { Id = c.Id, UserName = c.UserName, Name = c.Name, Password = c.Password, RoleId = c.RoleId.Value, Email = c.Email }).ToList();
            var userAutorize = HttpContext.User;
            var user = users.Where(_ => _.UserName == userAutorize.Identity.Name).FirstOrDefault();
            var products = _orderService.GetAll().Result.Where(_ => _.UsersId == user.Id).SelectMany(_ => _.Products).ToList();
            var role = _context.Roles.FirstOrDefault(_ => _.Id == user.RoleId);
            user.RoleName = role.Name;

            foreach (var t in users)
            {
                var rol = _context.Roles.FirstOrDefault(_ => _.Id == t.RoleId);
                t.RoleName = rol.Name;
            }

            var model = new CabinetModel
            {
                UserAutorize = user,
                Users = users,
                Products = products,
                SellerOrders = GetOrdersForSellerAsync(user.Id).Result,
                IsAdmin = HttpContext.User.IsInRole("admin"),
                IsModer = HttpContext.User.IsInRole("moder")
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Update(int id, UserModel model)
        {
            try
            {
                if (model != null)
                {
                    var users = _usersService.GetAllUsers().Result.Where(_ => _.Id == id).First();
                    var role = _context.Roles.Where(_ => _.Name == model.RoleName).Select(e => new Role() { Id = e.Id, Name = e.Name }).FirstOrDefault();
                    var roleUser = _context.Roles.Where(_ => _.Name == "user").Select(e => new Role() { Id = e.Id, Name = e.Name }).FirstOrDefault();

                    if (model.Email != null)
                        users.Email = model.Email;

                    if (model.Name != null)
                        users.Name = model.Name;

                    if (model.Password != null)
                        users.Password = model.Password;

                    if (roleUser != null)
                        users.Role = role != null ? role : roleUser;

                    _usersService.UpdateUser(users.Id, users.UserName, users.Name, users.Password, users.Role, users.Email);
                }

                return RedirectToAction("Cabinet");
            }
            catch
            {
                return RedirectToAction("Cabinet");
            }
        }


        public ActionResult Update(int id)
        {
            var model = new UserModel();
            var users = _usersService.GetAllUsers().Result.Where(_ => _.Id == id).First();
            var role = _context.Roles.Where(_ => _.Id == users.RoleId).Select(e => new Role() { Id = e.Id, Name = e.Name }).FirstOrDefault();
            var roles = _context.Roles.Select(r => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = r.Name,
                Text = r.Name
            })
            .ToList();

            if (users != null)
            {
                model.Id = users.Id;
                model.UserName = users.UserName;
                model.Name = users.Name;
                model.Password = users.Password;
                model.RoleName = role.Name;
                model.Email = users.Email;
                model.Roles = roles;
            }

            return PartialView("Update", model);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var model = new UserModel();
            model.Id = id;

            return PartialView("Delete", model);
        }


        [HttpPost]
        public ActionResult Delete(UserModel model)
        {
            var users = _usersService.GetAllUsers().Result.FirstOrDefault(_ => _.Id == model.Id);
            _usersService.DeleteUser(users.Id);

            return RedirectToAction("Cabinet");
        }

        public async Task<List<OrderSummary>> GetOrdersForSellerAsync(int sellerId)
        {
            List<OrderSummary> productsDb = _context.OrderProducts
                .Include(o => o.Product)
                .Where(_ => _.IsSold && _.Product.SellerId == sellerId)
                .GroupBy(op => op.Product.Name)
                .Select(group => new OrderSummary
                {
                    ProductName = group.Key,
                    Quantity = group.Sum(p => p.Quantity),
                    TotalAmount = group.Sum(p => p.Product.Price * p.Quantity)
                })
                .ToList();

            return productsDb;
        }

        public IActionResult GenerateExcel()
        {
            var userAutorize = HttpContext.User;
            var user = _usersService.GetAllUsers().Result.Where(_ => _.UserName == userAutorize.Identity.Name).FirstOrDefault();
            var orders = GetOrdersForSellerAsync(user.Id).Result;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Orders");

                worksheet.Cells[1, 1].Value = "Товар";
                worksheet.Cells[1, 2].Value = "Количество";
                worksheet.Cells[1, 3].Value = "Сумма";

                int row = 2;
                for (int i = 0; i < orders.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = orders[i].ProductName;
                    worksheet.Cells[i + 2, 2].Value = orders[i].Quantity;
                    worksheet.Cells[i + 2, 3].Value = orders[i].TotalAmount;
                    row++;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);

                stream.Position = 0;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"orders_report/{user.Id}.xlsx");
            }
        }
    }
}
