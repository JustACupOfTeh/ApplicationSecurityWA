using AceJobAgency.Models;
using AceJobAgency.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Web;

namespace AceJobAgency.Pages.Users
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<WebUser> userManager;
        private readonly SignInManager<WebUser> signInManager;
        private IWebHostEnvironment _environment;
        [BindProperty]
        public Register RModel { get; set; } = new();
        [BindProperty]
        public IFormFile? Resume { get; set; }
        public RegisterModel(UserManager<WebUser> userManager, SignInManager<WebUser> signInManager, IWebHostEnvironment _environment)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this._environment = _environment;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
                var protector = dataProtectionProvider.CreateProtector("MySecretKey");
                var user = new WebUser()
                {
                    UserName = RModel.Username,
                    FirstName = protector.Protect(HttpUtility.HtmlEncode(RModel.FirstName)),
                    LastName = protector.Protect(HttpUtility.HtmlEncode(RModel.LastName)),
                    Gender = protector.Protect(HttpUtility.HtmlEncode(RModel.Gender)),
                    NRIC = protector.Protect(HttpUtility.HtmlEncode(RModel.NRIC)),
                    Email = RModel.Email,
                    PasswordHash = RModel.Password,
                    DateOfBirth = RModel.BirthDate,
                    WhoamI = protector.Protect(RModel.WhoamI)

                };
                if (Resume != null)
                {
                    if (Resume.Length > 10 * 1024 * 1024)
                    {
                        ModelState.AddModelError("Resume", "File size cannot exceed 10MB");
                        return Page();
                    }
                    var uploadsFolder = "uploads";
                    var resumeFile = Guid.NewGuid() + Path.GetExtension(Resume.FileName);
                    var resumePath = Path.Combine(_environment.ContentRootPath, "wwwroot", uploadsFolder, resumeFile);
                    Directory.CreateDirectory(Path.GetDirectoryName(resumePath));
                    using var fileStream = new FileStream(resumePath, FileMode.Create);
                    await Resume.CopyToAsync(fileStream);
                    user.ResumeURL = string.Format("/{0}/{1}", uploadsFolder, resumeFile);
                }
                var result = await userManager.CreateAsync(user, RModel.Password);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);
                    return RedirectToPage("../Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return Page();
        }
        public void OnGet()
        {
        }
    }
}
