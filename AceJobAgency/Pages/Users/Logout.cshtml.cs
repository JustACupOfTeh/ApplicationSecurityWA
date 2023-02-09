using AceJobAgency.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AceJobAgency.Pages.Users
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<WebUser> signInManager;
        public LogoutModel(SignInManager<WebUser> signInManager)
        {
            this.signInManager = signInManager;
        }
        public void OnGet() { }
        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await signInManager.SignOutAsync();
            // HttpContext.Session.Clear();
            // HttpContext.Session.Remove("LoggedIn");
            return RedirectToPage("Login");
        }
        public async Task<IActionResult> OnPostDontLogoutAsync()
        {
            return RedirectToPage("../Index");
        }
    }
}
