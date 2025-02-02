using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OnlineMarket.API.Contacts;
using OnlineMarket.Application.Services;
using OnlineMarket.Core.Models;
using OnlineMarket.DataBase;
using OnlineMarket.DataBase.Entites;
using System.Security.Claims;

namespace OnlineMarket.API.Controllers
{
    public class AuthorizationController : Controller
    {

        private readonly MarketStoreDbContext _context;
        private readonly IUsersService _usersService;
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthorizationController(MarketStoreDbContext context, IUsersService service, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _usersService = service;
            _httpClientFactory = httpClientFactory;
        }

        [AllowAnonymous]
        public IActionResult ErrorLogin()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult History()
        {
            return View(HttpContext.User);
        }

        public IActionResult Figures()
        {
            return View(HttpContext.User);
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("MainBase", "Home");
        }

        [HttpPost]
        public bool CheckLogin([FromForm] UsersRequest user)
        {
            var item = _context.Users.Where(x => x.UserName == user.userName && x.Password == user.password).FirstOrDefault();
            if (item != null)
                return true;
            else
                return false;
        }

        [HttpPost]
        public async Task<bool> Registered([FromForm] UsersRequest user)
        {
            var item = _context.Users.Where(x => x.UserName == user.userName && x.Password == user.password || x.UserName == user.userName).FirstOrDefault();

            if (item != null)
                return false;

            var roleEntites = _context.Roles.FirstOrDefault(_ => _.Name == "user");

            var newUser = Users.Create(user.userName, user.name, user.password, roleEntites.Id, user.email).Users;
            await _usersService.CreateUser(newUser);

            if (newUser != null)
                return true;
            else
                return false;
        }

        private async Task Authenticate(UsersRequest user)
        {
            var item = _context.Users.Where(x => x.UserName == user.userName && x.Password == user.password).FirstOrDefault();
            var role = _context.Roles.Where(x => x.Id == item.RoleId).FirstOrDefault();

            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, item.UserName),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, role.Name)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        private async Task Authenticate(UsersEntity user)
        {
            var item = _context.Users.Where(x => x.UserName == user.UserName && x.Password == user.Password).FirstOrDefault();
            var role = _context.Roles.Where(x => x.Id == item.RoleId).FirstOrDefault();

            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, item.UserName),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, role.Name)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromForm] UsersRequest user)
        {
            bool IsSuccess = CheckLogin(user);
            if (IsSuccess)
            {
                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name,"ClaimName")
                    };

                var useridentity = new ClaimsIdentity(claims, "Login");
                ClaimsPrincipal principal = new ClaimsPrincipal(useridentity);


                await HttpContext.SignInAsync(principal);
                await Authenticate(user);

                return RedirectToAction("MainBase", "Home");
            }
            else if (user != null)
            {
                if (user.password == null)
                    ViewData["ErrorMessage"] = "Неверный пароль.";

                if (user.userName == null)
                    ViewData["ErrorMessage"] = "Неверный логин.";

                return View();
            }
            else
                return RedirectToAction("Register");
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register([FromForm] UsersRequest user)
        {
            bool IsSuccess = await Registered(user);

            if (IsSuccess)
            {
                if (string.IsNullOrEmpty(user.userName) || string.IsNullOrEmpty(user.password) || string.IsNullOrEmpty(user.name))
                {
                    ViewData["ErrorMessage"] = "Все поля должны быть заполнены.";
                    return View();
                }

                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name,"ClaimName")
                    };

                var useridentity = new ClaimsIdentity(claims, "Register");
                ClaimsPrincipal principal = new ClaimsPrincipal(useridentity);


                await HttpContext.SignInAsync(principal);

                return RedirectToAction("Login");
            }
            else
            {
                ViewData["ErrorMessage"] = "Пользователь с таким именем уже существует.";
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AuthenticateWithYandex(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }

            // Получение информации о пользователе с помощью токена
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.login.yandex.ru/info");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var userInfo = await response.Content.ReadAsStringAsync();
                dynamic user = JsonConvert.DeserializeObject(userInfo);

                // Получаем информацию о пользователе
                string userId = user.id;
                string userName = user.display_name;
                string email = user.default_email ?? "";

                // Проверяем, существует ли пользователь
                var existingUser = _context.Users.FirstOrDefault(x => x.UserName == userId);

                if (existingUser == null)
                {
                    var roleEntity = _context.Roles.FirstOrDefault(r => r.Name == "user");
                    var newUser = Users.Create(userId, userName, user.password, roleEntity.Id, email).Users;
                    await _usersService.CreateUser(newUser);
                    existingUser = newUser;
                }

                await Authenticate(existingUser);

                return RedirectToAction("MainBase", "Home");
            }
            else
            {
                ViewData["ErrorMessage"] = "Ошибка авторизации. Попробуйте снова.";
                return RedirectToAction("Login"); // Возвращаемся на страницу логина в случае ошибки
            }
        }
    }
}
