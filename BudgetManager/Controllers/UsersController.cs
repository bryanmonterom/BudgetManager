using BudgetManager.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BudgetManager.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public UsersController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [AllowAnonymous]
        public IActionResult Register()
        {

            return View();
        }
        [HttpPost]
        [AllowAnonymous]

        public async Task<IActionResult> Register(RegisterViewModel register)
        {

            if (!ModelState.IsValid)
            {
                return View(register);
            }

            var user = new User()
            {
                Email = register.Email,
            };

            var response = await userManager.CreateAsync(user, register.Password);

            if (response.Succeeded)
            {
                await signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Transactions");

            }

            foreach (var item in response.Errors)
            {
                ModelState.AddModelError(String.Empty, item.Description);
            }
            return View(register);
        }
        [AllowAnonymous]

        public IActionResult Login()
        {
            var model = new LoginViewModel();
            model.Email = "user1@email.com";
            model.Password = "Aa123456@";

            return View(model);
        }
        [AllowAnonymous]

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) {
                return View(model);
            }

            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded) {

                return RedirectToAction("Index", "Transactions");
            }
            ModelState.AddModelError(String.Empty, "The credentials provided are invalid");
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {

            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Transactions");
        }

    }
}