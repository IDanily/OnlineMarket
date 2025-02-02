using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OnlineMarket.Application.Services;
using OnlineMarket.Core.Abstractions;
using OnlineMarket.Core.Models;
using OnlineMarket.DataBase;
using OnlineMarket.DataBase.Entites;
using OnlineMarket.DataBase.Repositories;
using OnlineMarket.DataBase.Reposotories;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "wwwroot"
});
builder.Services.AddControllersWithViews();

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
    x =>
    {
        x.LoginPath = "/Authorization/Login";
        x.LogoutPath = "/Authorization/Logout";
        x.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Authorization/Login");
    });

builder.Services.AddMvc();
builder.Services.AddSingleton<TelegramService>();
builder.Services.AddScoped<IEntityService<Product>, ProductService>();
builder.Services.AddScoped<IEntityRepository<Product>, ProductRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IPriceComparison, PriceComparisonService>();
builder.Services.AddHttpClient();

builder.Services.AddDbContext<MarketStoreDbContext>(option =>
{
    option.UseSqlServer("workstation id=OnlineMarket.mssql.somee.com;packet size=4096;user id=danilklec_SQLLogin_1;pwd=bk99h1gtz5;data source=OnlineMarket.mssql.somee.com;persist security info=False;initial catalog=OnlineMarket;TrustServerCertificate=True");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        await InitializeDatabase(services, isDevelopment: true);
    }
}

app.UseCors(options =>
        options.WithOrigins("https://magazin.somee.com")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials());

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        if (exceptionHandlerPathFeature?.Error != null)
        {
            await context.Response.WriteAsync($"Error: {exceptionHandlerPathFeature.Error.Message}");
        }
    });
});
app.UseHsts();
app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Content-Security-Policy",
        "frame-ancestors 'self' https://autofill.yandex.ru https://magazin.somee.com");
    await next();
});

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseRouting();
app.MapGet("/", () => Results.Redirect("/Home/MainBase"));
app.MapControllerRoute(
    name: "Default",
    pattern: "{controller=Home}/{action=MainBase}"
    );

app.MapControllerRoute(
    name: "product",
    pattern: "Product/GetProduct/{id}",
    defaults: new { controller = "Product", action = "GetProduct" });

app.MapControllerRoute(
    name: "product",
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