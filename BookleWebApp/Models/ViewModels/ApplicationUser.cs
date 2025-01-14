using Microsoft.AspNetCore.Identity;

namespace BookleWebApp.Models.ViewModels
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
