using System.ComponentModel.DataAnnotations;

namespace BookleWebApp.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string? Email { get; set; }
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
