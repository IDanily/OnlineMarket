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

// ��������� �������������� � �������������� cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
    x =>
    {
        x.LoginPath = "/Authorization/Login";
        x.LogoutPath = "/Authorization/Logout";
        x.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Authorization/Login");
    });

// ���������� MVC
builder.Services.AddMvc();

// ���������� ��������
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

// ����������� � ���� ������
builder.Services.AddDbContext<MarketStoreDbContext>(option =>
{
    option.UseSqlServer("workstation id=OnlineMarket.mssql.somee.com;packet size=4096;user id=danilklec_SQLLogin_1;pwd=bk99h1gtz5;data source=OnlineMarket.mssql.somee.com;persist security info=False;initial catalog=OnlineMarket;TrustServerCertificate=True");
});

var app = builder.Build();

// ���������� ������������
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Content-Security-Policy",
        "frame-ancestors 'self' https://autofill.yandex.ru https://magazin.somee.com");
    await next();
});

// ����������� �����
app.UseStaticFiles();
// �������������
app.UseRouting();
// �������������� � �����������
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Redirect("/Home/MainBase"));

// �������� �������
app.MapControllerRoute(
    name: "Default",
    pattern: "{controller=Home}/{action=MainBase}"
    );

// ������� ��� ���������
app.MapControllerRoute(
    name: "product",
    pattern: "Product/GetProduct/{id}",
    defaults: new { controller = "Product", action = "GetProduct" });

// ������� ��� �������
app.MapControllerRoute(
    name: "order",
    pattern: "Order/GetOrder",
    defaults: new { controller = "Order", action = "GetOrder" });

app.Run();

static async Task InitializeDatabase(IServiceProvider serviceProvider, bool isDevelopment)
{
    var context = serviceProvider.GetRequiredService<MarketStoreDbContext>();

    // �������� �����
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

    // ������������ ������������ ��������� � �� �����
    var categoryMapping = new Dictionary<string, string>
    {
        { "�������", "Tech" },
        { "����������", "AutoGoods" },
        { "����������� �������", "AlcoholicBeverages" },
        { "�������", "Grocery" },
        { "������� �����", "HouseholdChemicals" },
        { "�����������", "Gastronomy" },
        { "������� ������", "ChildrensGoods" },
        { "������� � ����������� �������", "BabyDietFood" },
        { "������������ ��������, �������������", "FrozenFoods" },
        { "������������ �������", "Confectionery" },
        { "�����������", "CannedGoods" },
        { "�������� ������", "DairyProducts" },
        { "����, ����, �����, ����", "MeatFishPoultryEggs" },
        { "������� ��������������", "NonAlcoholicBeverages" },
        { "������������������� ������", "NonFoodGoods" },
        { "�����, ������, �����", "VegetablesFruitsNuts" },
        { "�������", "Plastic" },
        { "�������� �������, ������������", "Tobacco" },
        { "��������", "Textiles" },
        { "������ ��� ����, ��� ������", "Garden " },
        { "�����", "Hobbies" },
        { "���-����", "TeaCoffee" }
    };

    foreach (var categoryPair in categoryMapping)
    {
        var categoryName = categoryPair.Key;
        var categoryCode = categoryPair.Value;

        // �������� �� ������������� ��������� � ����� ���������
        var categoryExist = await context.Categories.AnyAsync(c => c.Name == categoryName);
        if (!categoryExist)
        {
            // ���������� ����� ��������� � �����
            context.Categories.Add(new CategoryEntity
            {
                Name = categoryName,
                Code = categoryCode // ��������� ���������� ���
            });
        }
    }

    await context.SaveChangesAsync();
}
