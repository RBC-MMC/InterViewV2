using InterViewV2.Models.VM;
using InterViewV2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using InterViewV2.Models.DAL;

namespace InterViewV2.Controllers
{
    public class AccountController : Controller
    {
        private readonly DB_Context _context;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AccountController(DB_Context context, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public IActionResult Login()
        {
            if (Request.Cookies.TryGetValue("Username", out string username))
            {
                var model = new LoginViewModel
                {
                    Username = username
                };
                return View(model);
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                if (model.RememberMe)
                {
                    Response.Cookies.Append("Username", model.Username, new CookieOptions
                    {
                        Expires = DateTime.UtcNow.AddDays(30)
                    });
                }
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ResetPassword(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            var model = new ResetPasswordViewModel { UserId = id };
            return View(model);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (changePasswordResult.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in changePasswordResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("Username");

            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
