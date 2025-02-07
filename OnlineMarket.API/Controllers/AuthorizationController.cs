using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

        private async Task Authenticate(UsersRequest user)
        {
            var item = _context.Users.Where(x => x.UserName == user.userName).FirstOrDefault();
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

        private async Task Authenticate(Users user)
        {
            var item = _context.Users.Where(x => x.UserName == user.UserName).FirstOrDefault();
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
            if (user == null || string.IsNullOrEmpty(user.userName) || string.IsNullOrEmpty(user.password))
            {
                ViewData["ErrorMessage"] = "Пожалуйста, заполните все поля.";
                return View();
            }

            var dbUser = _context.Users.Where(x => x.UserName == user.userName).FirstOrDefault();

            if (dbUser == null)
            {
                ViewData["ErrorMessage"] = "Неверный логин.";
                return View();
            }

            var passwordHasher = new PasswordHasher<UsersEntity>();
            var result = passwordHasher.VerifyHashedPassword(dbUser, dbUser.Password, user.password);

            if (result == PasswordVerificationResult.Failed)
            {
                ViewData["ErrorMessage"] = "Неверный пароль.";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, dbUser.UserName)
            };

            var useridentity = new ClaimsIdentity(claims, "Login");
            ClaimsPrincipal principal = new ClaimsPrincipal(useridentity);

            await HttpContext.SignInAsync(principal);
            await Authenticate(user);

            return RedirectToAction("MainBase", "Home");
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register([FromForm] UsersRequest user)
        {
            if (string.IsNullOrEmpty(user.userName) || string.IsNullOrEmpty(user.password) || string.IsNullOrEmpty(user.name))
            {
                ViewData["ErrorMessage"] = "Все поля должны быть заполнены.";
                return View();
            }

            var existingUser = _context.Users.FirstOrDefault(u => u.UserName == user.userName);
            if (existingUser != null)
            {
                ViewData["ErrorMessage"] = "Пользователь с таким именем уже существует.";
                return View();
            }

            var passwordHasher = new PasswordHasher<Users>();
            string hashedPassword = passwordHasher.HashPassword(null, user.password);
            var userRole = _context.Roles.FirstOrDefault(_ => _.Name == "user");

            var newUser = new Users
            {
                UserName = user.userName,
                Password = hashedPassword,
                Name = user.name,
                Email = user.email,
                RoleId = userRole.Id,
            };

            await _usersService.CreateUser(newUser);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.userName)
            };

            var useridentity = new ClaimsIdentity(claims, "Register");
            ClaimsPrincipal principal = new ClaimsPrincipal(useridentity);

            await HttpContext.SignInAsync(principal);

            return RedirectToAction("Login");
        }

        [HttpGet("YandexTokenRedirect")]
        [AllowAnonymous]
        public IActionResult YandexTokenRedirect()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AuthenticateWithYandex(string access_token)
        {
            if (string.IsNullOrEmpty(access_token))
                return BadRequest("access_token пустой");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.GetAsync($"https://login.yandex.ru/info?oauth_token={access_token}");
                if (response.IsSuccessStatusCode)
                {
                    string userInfo = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(userInfo))
                    {
                        dynamic user = JsonConvert.DeserializeObject(userInfo);

                        if (user != null)
                        {
                            // Получаем информацию о пользователе
                            string userName = user.real_name;
                            string userLogin = user.login;
                            string email = user.default_email ?? "";
                            // Хэшируем пароль
                            var passwordHasher = new PasswordHasher<Users>();
                            string hashedPassword = passwordHasher.HashPassword(null, "1111");

                            // Проверяем, существует ли пользователь
                            var existingUser = _context.Users.Where(x => x.UserName == userLogin).Select(_ => new Users { Id = _.Id, Email = _.Email, Name = _.Name, Password = _.Password, RoleId = _.RoleId, UserName = _.UserName }).FirstOrDefault();

                            if (existingUser == null)
                            {
                                var roleEntity = _context.Roles.FirstOrDefault(r => r.Name == "user");
                                var newUser = Users.Create(userLogin, userName, hashedPassword, roleEntity.Id, email).Users;
                                await _usersService.CreateUser(newUser);
                                existingUser = newUser;
                            }
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, existingUser.UserName)
                            };

                            var useridentity = new ClaimsIdentity(claims, "Login");
                            ClaimsPrincipal principal = new ClaimsPrincipal(useridentity);

                            await HttpContext.SignInAsync(principal);

                            await Authenticate(existingUser);

                            return RedirectToAction("MainBase", "Home");
                        }

                        return BadRequest("Десериализация не прошла");
                    }

                    return BadRequest("Яндекс вернул пустоту");
                }
                else
                {
                    return BadRequest("Ошибка авторизации");
                }
            }
        }
    }
}
