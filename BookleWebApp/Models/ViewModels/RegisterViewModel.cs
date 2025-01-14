using System.ComponentModel.DataAnnotations;

namespace BookleWebApp.Models.ViewModels
{
    public class RegisterViewModel
    {
        public string FullName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirm not match")]
        public string ConfirmPassword { get; set; }

    }
}
