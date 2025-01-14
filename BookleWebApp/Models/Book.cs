using BookleWebApp.Models.CommonProp;
using BookleWebApp.Models.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookleWebApp.Models
{
    public class Book : SharedProp
    {
        [Key]
        public int Id { get; set; } 
        public string Title { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int StockQuantity { get; set; }
        public string BookLanguage { get; set; }
        public string? BookImage { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
        public int CategoryId { get; set; } 
        public Category? Category { get; set; }
        public string? PublisherId { get; set; }
        public Publisher? Publisher { get; set; }
        
        public ICollection<Review>? Reviews { get; set; }
    }
}
