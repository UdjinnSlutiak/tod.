using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using tod.Models;
using tod.Models.ViewModels;

namespace tod.Controllers
{
    public class AuthorizationController : Controller
    {

        private readonly topxDbContext context;

        public AuthorizationController(topxDbContext context)
        {
            this.context = context;
        }

        public IActionResult Main() => View();

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User
                {
                    Nickname = model.Nickname,
                    Email = model.Email,
                    Password = model.Password
                };

                List<User> usersByNickname = context.Users.Where(u => u.Nickname == user.Nickname).ToList();
                List<User> usersByEmail = context.Users.Where(u => u.Email == user.Email).ToList();

                if (usersByNickname.Count == 0 && usersByEmail.Count == 0)
                {
                    context.Users.Add(user);
                    await context.SaveChangesAsync();
                    await Authenticate(model.Nickname);
                    return RedirectToAction("Main", "Home");
                }
                if (usersByNickname.Count != 0)
                {
                    ModelState.AddModelError("Nickname", "User with such nickname already exists");
                }
                if (usersByEmail.Count != 0)
                {
                    ModelState.AddModelError("Email", "User with such email already exists");
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {

            if (ModelState.IsValid)
            {
                User user = new User()
                {
                    Nickname = model.Nickname,
                    Password = model.Password
                };

                User userByInfo = context.Users.Where(u => u.Nickname == user.Nickname && u.Password == user.Password).SingleOrDefault();

                if (userByInfo != null)
                {
                    await Authenticate(model.Nickname);
                    return RedirectToAction("Main", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Nickname or password is incorrect");
                }
            }

            return View(model);

        }

        public async Task Authenticate(string userName)
        {

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));

        }

        public async Task<IActionResult> Logout()
        {

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Main", "Authorization");

        }

    }
}
