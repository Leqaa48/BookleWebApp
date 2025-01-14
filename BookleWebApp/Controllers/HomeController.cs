using System.Diagnostics;
using BookleWebApp.Data;
using BookleWebApp.Models;
using BookleWebApp.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookleWebApp.Controllers
{
    public class HomeController : Controller
    {

        private UserManager<ApplicationUser> userManager;
        private SignInManager<ApplicationUser> signInManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;

        public HomeController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> _signInManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext context)
        {
            this.userManager = userManager;
            signInManager = _signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Get the current logged-in user
            var currentUser = await userManager.GetUserAsync(User);

            // Retrieve the books list from the database
            var books = _context.Books.ToList();

            // Calculate the number of items in the cart for the logged-in user
            int cartItemCount = 0;
            if (currentUser != null)
            {
                cartItemCount = await _context.Carts
                                              .Where(c => c.UserId == currentUser.Id)
                                              .SelectMany(c => c.CartItems)
                                              .CountAsync();
            }

            // Pass the cart count to the ViewData
            ViewData["CartCount"] = cartItemCount;

            // Return the books list to the view
            return View(books);
        }

        public IActionResult AboutUs()
        {
            return View();
        }
        public IActionResult Blog()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
