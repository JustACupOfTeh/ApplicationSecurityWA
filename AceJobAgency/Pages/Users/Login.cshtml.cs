using AceJobAgency.Models;
using AceJobAgency.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Server.IIS.Core;
using System.Web;

namespace AceJobAgency.Pages.Users
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<WebUser> signInManager;
        private readonly UserManager<WebUser> userManager;
        
        [BindProperty]
        public Login LModel { get; set; }
        public LoginModel(SignInManager<WebUser> signInManager, UserManager<WebUser> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                
                var user = await userManager.FindByEmailAsync(LModel.Email);
                if (!await userManager.IsLockedOutAsync(user))
                {
                    if (user != null)
                    {
                        if (signInManager.IsSignedIn(User))
                        {
                            await signInManager.SignOutAsync();
                            HttpContext.Session.Remove("LoggedIn");
                            HttpContext.Session.Remove("AuthToken");
                        }
                        var identityResult = await signInManager.PasswordSignInAsync(
                            user.UserName,
                            LModel.Password,
                            LModel.RememberMe,
                            false
                            );
                        if (identityResult.Succeeded)
                        {
                            string guid = Guid.NewGuid().ToString();

                            HttpContext.Session.SetString("LoggedIn", user.UserName);
                            HttpContext.Session.SetString("AuthToken", guid);
                            Response.Cookies.Append("AuthToken", guid, new CookieOptions
                            {
                                Expires = DateTime.Now.AddMinutes(30)
                            });
                            await userManager.ResetAccessFailedCountAsync(user);
                            return RedirectToPage("../Index");
                        }
                        ModelState.AddModelError("", "Email or Password incorrect");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Account has been locked out due to too many failed login attempts");
                }
                
            }
            return Page();
        }
        public void OnGet()
        {
        }
    }
}
