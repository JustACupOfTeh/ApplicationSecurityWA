using AceJobAgency.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AceJobAgency.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public User CurrentUser = new();
        public User EncryptedUser = new();
        private readonly ILogger<IndexModel> _logger;
        private readonly SignInManager<WebUser> signInManager;
        private readonly UserManager<WebUser> userManager;

        public IndexModel(ILogger<IndexModel> logger, SignInManager<WebUser> signInManager, UserManager<WebUser> userManager)
        {
            _logger = logger;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (signInManager.IsSignedIn(User) && HttpContext.Session.GetString("LoggedIn") != null
                && HttpContext.Session.GetString("AuthToken") != null && Request.Cookies["AuthToken"] != null)
            {
                if (!HttpContext.Session.GetString("AuthToken").ToString().Equals(Request.Cookies["AuthToken"]))
                {
                    HttpContext.Session.Remove("LoggedIn");
                    HttpContext.Session.Remove("AuthToken");
                    Response.Cookies.Delete("AuthToken");
                    return RedirectToPage("Users/Login");
                }
                var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
                var protector = dataProtectionProvider.CreateProtector("MySecretKey");
                var user = await userManager.GetUserAsync(User);
                if (user != null)
                {
                    CurrentUser.Username = user.UserName;
                    CurrentUser.FirstName = protector.Unprotect(user.FirstName);
                    CurrentUser.LastName = protector.Unprotect(user.LastName);
                    CurrentUser.Gender = protector.Unprotect(user.Gender);
                    CurrentUser.NRIC = protector.Unprotect(user.NRIC);
                    CurrentUser.DateOfBirth = user.DateOfBirth;
                    CurrentUser.Email = user.Email;
                    CurrentUser.WhoamI = protector.Unprotect(user.WhoamI);
                    CurrentUser.ResumeURL = user.ResumeURL;
                    EncryptedUser.Username = user.UserName;
                    EncryptedUser.FirstName = user.FirstName;
                    EncryptedUser.LastName = user.LastName;
                    EncryptedUser.Gender = user.Gender;
                    EncryptedUser.NRIC = user.NRIC;
                    EncryptedUser.DateOfBirth = user.DateOfBirth;
                    EncryptedUser.Email = user.Email;
                    EncryptedUser.WhoamI = user.WhoamI;
                    EncryptedUser.ResumeURL = user.ResumeURL;
                }
                return Page();
            }
            else
            {
                await signInManager.SignOutAsync();
                return RedirectToPage("Users/Login");
            }
        }
    }
}