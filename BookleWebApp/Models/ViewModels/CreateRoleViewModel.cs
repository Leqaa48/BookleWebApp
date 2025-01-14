using System.ComponentModel.DataAnnotations;

namespace BookleWebApp.Models.ViewModels
{
    public class CreateRoleViewModel
    {
            [Required]
            public string RoleName { get; set; }
        
    }
}
