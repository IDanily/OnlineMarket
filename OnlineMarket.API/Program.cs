using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using OnlineMarket.Application.Services;
using OnlineMarket.Core.Abstractions;
using OnlineMarket.Core.Models;
using OnlineMarket.DataBase;
using OnlineMarket.DataBase.Entites;
using OnlineMarket.DataBase.Repositories;
using OnlineMarket.DataBase.Reposotories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Настройка аутентификации с использованием cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
    x =>
    {
        x.LoginPath = "/Authorization/Login";
        x.LogoutPath = "/Authorization/Logout";
        x.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Authorization/Login");
    });

// Добавление MVC
builder.Services.AddMvc();

// Добавление сервисов
builder.Services.AddSingleton<TelegramService>();
builder.Services.AddScoped<IEntityService<Product>, ProductService>();
builder.Services.AddScoped<IEntityRepository<Product>, ProductRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IPriceComparison, PriceComparisonService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddHttpClient();

// Подключение к базе данных
builder.Services.AddDbContext<MarketStoreDbContext>(option =>
{
    option.UseSqlServer("workstation id=OnlineMarket.mssql.somee.com;packet size=4096;user id=danilklec_SQLLogin_1;pwd=bk99h1gtz5;data source=OnlineMarket.mssql.somee.com;persist security info=False;initial catalog=OnlineMarket;TrustServerCertificate=True");
});

var app = builder.Build();

// Контентная безопасность
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Content-Security-Policy",
        "frame-ancestors 'self' https://autofill.yandex.ru https://magazin.somee.com");
    await next();
});

// Статические файлы
app.UseStaticFiles();
// Маршрутизация
app.UseRouting();
// Аутентификация и авторизация
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Redirect("/Home/MainBase"));

// Основной маршрут
app.MapControllerRoute(
    name: "Default",
    pattern: "{controller=Home}/{action=MainBase}"
    );

// Маршрут для продуктов
app.MapControllerRoute(
    name: "product",
    pattern: "Product/GetProduct/{id}",
    defaults: new { controller = "Product", action = "GetProduct" });

// Маршрут для заказов
app.MapControllerRoute(
    name: "order",
    pattern: "Order/GetOrder",
    defaults: new { controller = "Order", action = "GetOrder" });

app.Run();

static async Task InitializeDatabase(IServiceProvider serviceProvider, bool isDevelopment)
{
    var context = serviceProvider.GetRequiredService<MarketStoreDbContext>();

    // Проверка ролей
    if (!context.Roles.Any())
    {
        var roles = new[]
        {
            new RoleEntity { Name = "admin" },
            new RoleEntity { Name = "user" },
            new RoleEntity { Name = "seller" }
        };
        context.Roles.AddRange(roles);
    }

    // Соответствие наименований категорий и их кодов
    var categoryMapping = new Dictionary<string, string>
    {
        { "Техника", "Tech" },
        { "Автотовары", "AutoGoods" },
        { "Алкогольные напитки", "AlcoholicBeverages" },
        { "Бакалея", "Grocery" },
        { "Бытовая химия", "HouseholdChemicals" },
        { "Гастрономия", "Gastronomy" },
        { "Детские товары", "ChildrensGoods" },
        { "Детское и диетическое питание", "BabyDietFood" },
        { "Замороженные продукты, полуфабрикаты", "FrozenFoods" },
        { "Кондитерские изделия", "Confectionery" },
        { "Консервация", "CannedGoods" },
        { "Молочная группа", "DairyProducts" },
        { "Мясо, рыба, птица, яйцо", "MeatFishPoultryEggs" },
        { "Напитки безалкогольные", "NonAlcoholicBeverages" },
        { "Непродовольственные товары", "NonFoodGoods" },
        { "Овощи, фрукты, орехи", "VegetablesFruitsNuts" },
        { "Пластик", "Plastic" },
        { "Табачные изделия, контрацепция", "Tobacco" },
        { "Текстиль", "Textiles" },
        { "Товары для дачи, для отдыха", "Garden " },
        { "Хобби", "Hobbies" },
        { "Чай-кофе", "TeaCoffee" }
    };

    foreach (var categoryPair in categoryMapping)
    {
        var categoryName = categoryPair.Key;
        var categoryCode = categoryPair.Value;

        // Проверка на существование категории с таким названием
        var categoryExist = await context.Categories.AnyAsync(c => c.Name == categoryName);
        if (!categoryExist)
        {
            // Добавление новой категории с кодом
            context.Categories.Add(new CategoryEntity
            {
                Name = categoryName,
                Code = categoryCode // Добавляем уникальный код
            });
        }
    }

    await context.SaveChangesAsync();
}
