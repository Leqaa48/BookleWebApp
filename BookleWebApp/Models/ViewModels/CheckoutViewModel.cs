using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookleWebApp.Models.ViewModels
{
    public class CheckoutViewModel
    {
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string Country { get; set; }
        public string UserAddress { get; set; }
        public string UserAddress2 { get; set; }
        public string TownCity { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Notes { get; set; }
        public string PaymentMethod { get; set; }
        public List<CartItem> CartItems { get; set; }
    }
}
