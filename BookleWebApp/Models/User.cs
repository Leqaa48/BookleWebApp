using BookleWebApp.Models.CommonProp;
using System.ComponentModel.DataAnnotations;

namespace BookleWebApp.Models
{
    public class User : SharedProp
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}
