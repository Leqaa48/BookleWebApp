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

namespace BookleWebApp.Controllers
{
    [Authorize(Roles = "User")]
    public class OrdersController : Controller
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Orders.Include(o => o.User);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public async Task<IActionResult> Create()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string userId = userIdClaim.Value;

            // Fetch the cart items for the logged-in user
            var cartItems = await _context.CartItems
                                           .Where(c => c.Cart.UserId == userId)  // Make sure to match the userId to Cart's UserId
                                           .Include(c => c.Book)
                                           .ToListAsync();

            if (cartItems == null || !cartItems.Any())
            {
                // Handle case where cart is empty or doesn't exist
                ModelState.AddModelError("", "Your cart is empty or does not exist.");
                return RedirectToAction("Index", "Cart");  // Or redirect to another view
            }

            // Create an order model to pass to the view
            var order = new Order
            {
                OrderDate = DateTime.Now,
                UserId = userId,
                OrderItems = cartItems.Select(c => new OrderItem
                {
                    BookId = c.BookId,
                    Book = c.Book,
                    Quantity = c.Quantity,
                    UnitPrice = c.Book.Price
                }).ToList(),
                TotalAmount = cartItems.Sum(c => c.Quantity * c.Book.Price),
                Status = Order.OrderStatus.Pending
            };

            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userId);  // Set the user in the dropdown
            return View(order);  // Return the order view with cart items prefilled
        }


        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string userId = userIdClaim.Value;

            // Get the user's cart based on the UserId
            var cart = await _context.Carts
                .Include(c => c.CartItems)  // Include CartItems in the Cart
                .ThenInclude(ci => ci.Book)  // Include Book for each CartItem
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                // If no cart is found or the cart is empty, return an error or redirect
                ModelState.AddModelError("", "Your cart is empty or does not exist.");
                return View(order); // Return the same view with an error message
            }

            // Create a new order object with details from the cart
            order.UserId = userId;
            order.OrderDate = DateTime.Now;
            order.TotalAmount = cart.TotalAmount; // Calculate from cart total
            order.Status = Order.OrderStatus.Pending;
            order.Notes = order.Notes ?? "";

            // Add the order to the context
            _context.Add(order);
            await _context.SaveChangesAsync();

            // Create OrderItems based on CartItems
            foreach (var cartItem in cart.CartItems)
            {
                var book = cartItem.Book;
                if (book.StockQuantity < cartItem.Quantity)
                {
                    // If stock is insufficient, handle the error
                    TempData["ErrorMessage"] = $"Insufficient stock for {book.Title}.";
                    return RedirectToAction("Details", "Carts");
                }

                // Reduce stock quantity
                book.StockQuantity -= cartItem.Quantity;

                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    BookId = cartItem.BookId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Book.Price
                };

                _context.Add(orderItem);
            }

            // Save the OrderItems and update book stocks in the database
            await _context.SaveChangesAsync();

            // Optionally, clear the cart after successful checkout
            _context.Carts.Remove(cart); // Clear the cart
            await _context.SaveChangesAsync();

            // Set success message to TempData
            TempData["SuccessMessage"] = "Your order has been placed successfully!";

            // Redirect to the order details page
            return RedirectToAction("Details", "Carts");
        }



        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", order.UserId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,OrderDate,Address,TotalAmount,Notes,Status")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", order.UserId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
