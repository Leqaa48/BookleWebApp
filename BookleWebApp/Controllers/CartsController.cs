using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookleWebApp.Data;
using BookleWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BookleWebApp.Controllers
{
    [Authorize(Roles = "User")]
    public class CartsController : Controller
    {
        private readonly AppDbContext _context;

        public CartsController(AppDbContext context)
        {
            _context = context;
        }

        // Add item to cart
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCart(int bookId)
        {
            // Retrieve the logged-in user ID
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string userId = userIdClaim.Value;

            // Ensure the book exists
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                return NotFound("The specified book does not exist.");
            }

            // Retrieve or create the user's cart
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CartItems = new List<CartItem>()
                };
                _context.Carts.Add(cart);
            }

            // Check if the book is already in the cart
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.BookId == bookId);
            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    BookId = bookId,
                    Quantity = 1,
                    CartId = cart.CartId
                };
                cart.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity++;
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Redirect to cart details
            return RedirectToAction("Details", new { id = cart.CartId });
        }

        // Show user's cart
        [Authorize]
        public async Task<IActionResult> Cart()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string userId = userIdClaim.Value;

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CartItems = new List<CartItem>()
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return View(cart);
        }

        // Details of a specific cart
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            Cart cart;

            if (id != null)
            {
                // Attempt to find the cart by its ID
                cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Book)
                    .FirstOrDefaultAsync(c => c.CartId == id);
            }
            else
            {
                // Get the logged-in user's ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Fetch the cart for the logged-in user
                cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Book)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                // If no cart exists for the user, create a new one
                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserId = userId,
                        CartItems = new List<CartItem>()
                    };

                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync();
                }
            }

            // Ensure a cart exists to pass to the view
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }


        // Remove item from cart
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Details", new { id = cartItem.CartId });
        }

        // Decrease item quantity in cart
        [HttpPost]
        public async Task<IActionResult> DecreaseQuantity(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem != null && cartItem.Quantity > 1)
            {
                cartItem.Quantity--;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Details", new { id = cartItem.CartId });
        }

        // Increase item quantity in cart
        [HttpPost]
        public async Task<IActionResult> IncreaseQuantity(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem != null)
            {
                cartItem.Quantity++;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Details", new { id = cartItem.CartId });
        }

        // Clear all items from the cart
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ClearCart()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string userId = userIdClaim.Value;
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null)
            {
                _context.CartItems.RemoveRange(cart.CartItems);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Cart");
        }

        // Checkout page
        [Authorize]
        public IActionResult Checkout()
        {
            // Checkout logic
            return View();
        }

        // GET: Carts/Index
        public async Task<IActionResult> Index()
        {
            return View(await _context.Carts.ToListAsync());
        }

        // Create cart (used for initial setup)
        public IActionResult Create()
        {
            return View();
        }

        // POST: Carts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CartId,UserId")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cart);
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            return View(cart);
        }

        // POST: Carts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CartId,UserId")] Cart cart)
        {
            if (id != cart.CartId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.CartId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cart);
        }

        // GET: Carts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .FirstOrDefaultAsync(m => m.CartId == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int id)
        {
            return _context.Carts.Any(e => e.CartId == id);
        }
    }
}
