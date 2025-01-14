using BookleWebApp.Models.CommonProp;

namespace BookleWebApp.Models
{
    public class Publisher : SharedProp
    {
        public String Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public ICollection<Book>? PublishedBooks { get; set; }
    }
}

