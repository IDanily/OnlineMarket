using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineMarket.API.Models;
using OnlineMarket.API.ViewModels;
using OnlineMarket.Application.Services;
using OnlineMarket.Core.Abstractions;
using OnlineMarket.Core.Models;
using OnlineMarket.DataBase;

namespace OnlineMarket.API.Controllers
{
    [AllowAnonymous]
    public class ProductController(MarketStoreDbContext context, IEntityService<Product> productService, IUsersService service, IWebHostEnvironment webHostEnvironment, IPriceComparison priceService) : Controller
    {
        private readonly MarketStoreDbContext _context = context;
        private readonly IEntityService<Product> _productService = productService;
        private readonly IUsersService _usersService = service;
        private readonly IPriceComparison _priceService = priceService;
        private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

        [Authorize(Roles = "admin,seller")]
        public IActionResult ProductList(ProductFilterViewModel filter)
        {
            var userAutorize = HttpContext.User;
            var user = _context.Users.FirstOrDefault(_ => _.UserName == userAutorize.Identity.Name);
            var products = _context.Products.Include(_ => _.Category).Where(_ => _.SellerId == user.Id).ToList();

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
                IsSeller = HttpContext.User.IsInRole("seller"),
                Filter = filter
            };

            ViewData["SearchQuery"] = filter.Query;

            return View(model);
        }

        #region Обновление
        [HttpPost]
        public async Task<ActionResult> UpdateProduct(int id, ProductsActionModel productRequest)
        {
            if (productRequest != null)
            {
                var product = await _context.Products.FirstOrDefaultAsync(_ => _.Id == id);

                if (product == null)
                    return BadRequest("Продукт не найден");

                if (productRequest.Number != null)
                    product.Number = productRequest.Number;

                product.Price = productRequest.Price;

                if (productRequest.Name != null)
                    product.Name = productRequest.Name;

                if (productRequest.Description != null)
                    product.Description = productRequest.Description;

                if (productRequest.Picture != null)
                {
                    if (productRequest.ExistingImage != null)
                    {
                        string existingFilePath = Path.Combine(_webHostEnvironment.WebRootPath, FileLocation.FileUploadFolder, productRequest.ExistingImage);

                        if (System.IO.File.Exists(existingFilePath))
                        {
                            System.IO.File.Delete(existingFilePath);
                        }
                    }

                    product.Picture = ProcessUploadedFile(productRequest);
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ProductList", "Product");
        }


        public ActionResult UpdateProduct(int id)
        {
            var model = new ProductsActionModel();
            var product = _context.Products.Where(_ => _.Id == id).FirstOrDefault();

            if (product != null)
            {
                model.Id = product.Id;
                model.Number = product.Number;
                model.Name = product.Name;
                model.Description = product.Description;
                model.ExistingImage = product.Picture;
                model.Price = product.Price;
            }

            return PartialView("UpdateProduct", model);
        }
        #endregion

        #region Создание
        [AllowAnonymous]
        public IActionResult Create()
        {
            var products = _productService.GetAll().Result;
            var userAutorize = HttpContext.User;

            var model = new ProductsListingModel
            {
                User = userAutorize,
                Products = products,
                IsAdmin = HttpContext.User.IsInRole("admin")
            };

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<int>> Create([FromForm] ProductsActionModel model)
        {
            var userAutorize = HttpContext.User;
            var user = _context.Users.Where(_ => _.UserName == userAutorize.Identity.Name).Select(c => new Users() {Id = c.Id, UserName = c.UserName, Password = c.Password, RoleId = c.RoleId }).FirstOrDefault();
            string uniqueFileName = ProcessUploadedFile(model);
            var category = _context.Categories.Where(_ => _.Id == model.CategoryId).Select(c => new Category() { Id = c.Id, Code = c.Code, DateCreate = c.DateCreate, DateExpiration = c.DateExpiration, Description = c.Description, Name = c.Name }).FirstOrDefault();

            var (product, error) = Product.Create(
                model.Id,
                model.Number,
                model.Description,
                model.Name,
                uniqueFileName,
                model.Price,
                category,
                user);

            if (!string.IsNullOrEmpty(error))
            {
                return BadRequest(error);
            }

            var productId = await _productService.Create(product);

            return RedirectToAction("MainBase", "Home");
        }
        #endregion

        #region Удаление
        [HttpGet]
        public ActionResult DeleteProduct(int id)
        {
            var model = new ProductsActionModel();
            model.Id = id;

            return PartialView("DeleteProduct", model);
        }

        [HttpPost]
        public ActionResult DeleteProduct(ProductsActionModel model)
        {
            var product = _productService.Get(model.Id).Result;
            var productId = _productService.Delete(product.Id);

            return RedirectToAction("MainBase", "Home");
        }
        #endregion

        [HttpGet]
        public async Task<ActionResult> GetProductAsync(int id)
        {
            var model = new ProductsActionModel();

            var product = _productService.Get(id).Result;
            model.Id = product.Id;
            model.Number = product.Number;
            model.Name = product.Name;
            model.Description = product.Description;
            model.CategoryName = product.CategoryName;
            model.Price = product.Price;
            model.PicturePath = product.Picture;

            model.CompetitorPrices = await _priceService.GetCompetitorPricesAsync(product.Name);

            return View("GetProduct", model);
        }

        private string ProcessUploadedFile(ProductsActionModel entity)
        {
            string uniqueFileName = "";

            if (entity.Picture != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, FileLocation.FileUploadFolder);
                uniqueFileName = entity.Id + "_" + entity.Picture.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var fileStream = new FileStream(filePath, FileMode.Create);
                entity.Picture.CopyTo(fileStream);
            }

            return uniqueFileName;
        }

    }
}

