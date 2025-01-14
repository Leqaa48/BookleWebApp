using System.ComponentModel.DataAnnotations.Schema;

namespace BookleWebApp.Models
{
    public class Category
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public string? Image { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }

        public ICollection<Book>? Books { get; set; } 
    }
}
