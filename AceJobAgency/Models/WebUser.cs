using Microsoft.AspNetCore.Identity;

namespace AceJobAgency.Models
{
    public class WebUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string NRIC { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? ResumeURL { get; set; }
        public string WhoamI { get; set; }
    }
}
