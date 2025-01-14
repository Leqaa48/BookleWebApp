namespace BookleWebApp.Models
{
    public class Review 
    {
        public int Id { get; set; } 
        public string UserId { get; set; }
        public User User { get; set; } 
        public int BookId { get; set; }
        public Book Book { get; set; } 
        public string Content { get; set; } 
        public int Rating { get; set; } 
        public DateTime CreatedAt { get; set; } 

    }
}
