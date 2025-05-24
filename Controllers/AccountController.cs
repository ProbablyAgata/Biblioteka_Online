using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BibliotekaOnline.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace BibliotekaOnline.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            // Redirect to the Identity login page
            return RedirectToPage("/Account/Login", new { area = "Identity", returnUrl });
        }

        [HttpGet]
        public IActionResult Register()
        {
            // Redirect to the Identity register page
            return RedirectToPage("/Account/Register", new { area = "Identity" });
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
} 