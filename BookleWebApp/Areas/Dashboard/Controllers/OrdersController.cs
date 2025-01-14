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

namespace BookleWebApp.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Dashboard/Orders
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Orders.Include(o => o.User);
            return View(await appDbContext.ToListAsync());
        }
        public async Task<IActionResult> Completed(int? id)
        {
            // Fetch orders including user data
            var appDbContext = _context.Orders.Include(o => o.User);

            // If no specific ID is provided, show completed orders
            if (id == null)
            {
                var completedOrders = await appDbContext
                    .Where(o => o.Status == Order.OrderStatus.Completed)
                    .ToListAsync();
                return View(completedOrders);
            }

            // Fetch the order with the specified ID
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            // Update order status if ModelState is valid
            if (ModelState.IsValid)
            {
                try
                {
                    order.Status = Order.OrderStatus.Completed;
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
            }

            // Fetch and display completed orders
            var completedOrdersList = await appDbContext
                .Where(o => o.Status == Order.OrderStatus.Completed)
                .ToListAsync();
            return View(completedOrdersList);
        }
        public async Task<IActionResult> Pending(int? id)
        {
            // Fetch orders including user data
            var appDbContext = _context.Orders.Include(o => o.User);

            // If no specific ID is provided, show pending orders
            if (id == null)
            {
                var pendingOrders = await appDbContext
                    .Where(o => o.Status == Order.OrderStatus.Pending)
                    .ToListAsync();

                // Pass the list of pending orders to the view
                return View(pendingOrders);
            }

            // Fetch the order with the specified ID
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            // Update order status if ModelState is valid
            if (ModelState.IsValid)
            {
                try
                {
                    order.Status = Order.OrderStatus.Pending;
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Orders.Any(o => o.Id == order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Fetch and display pending orders after the update
            var pendingOrdersList = await appDbContext
                .Where(o => o.Status == Order.OrderStatus.Pending)
                .ToListAsync();
            return View(pendingOrdersList);
        }

        public async Task<IActionResult> Cancelled(int? id)
        {
            // Fetch orders including user data
            var appDbContext = _context.Orders.Include(o => o.User);

            // If no specific ID is provided, show completed orders
            if (id == null)
            {
                var CancelledOrders = await appDbContext
                    .Where(o => o.Status == Order.OrderStatus.Cancelled)
                    .ToListAsync();
                return View(CancelledOrders);
            }

            // Fetch the order with the specified ID
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            // Update order status if ModelState is valid
            if (ModelState.IsValid)
            {
                try
                {
                    order.Status = Order.OrderStatus.Cancelled;
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
            }

            // Fetch and display completed orders
            var CancelledOrdersList = await appDbContext
                .Where(o => o.Status == Order.OrderStatus.Cancelled)
                .ToListAsync();
            return View(CancelledOrdersList);
        }

        // GET: Dashboard/Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.User) // Include user information
                .Include(o => o.OrderItems) // Include order items
                .ThenInclude(oi => oi.Book) // Include book details for order items
                .ThenInclude(b => b.Publisher) // Include publisher details for books
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }




        // GET: Dashboard/Orders/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName");
            return View();
        }

        // POST: Dashboard/Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,OrderDate,Address,City,TotalAmount,Notes,Status")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", order.UserId);
            return View(order);
        }

        // GET: Dashboard/Orders/Edit/5
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

            // Pass the OrderStatus options to the view
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", order.UserId);
            ViewBag.OrderStatus = Enum.GetValues(typeof(Order.OrderStatus))
                                           .Cast<Order.OrderStatus>()
                                           .Select(s => new { Value = s, Text = s.ToString() }).ToList();

            return View(order);
        }


        // POST: Dashboard/Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,OrderDate,Address,City,TotalAmount,Notes,Status")] Order order)
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", order.UserId);
            return View(order);
        }

        // GET: Dashboard/Orders/Delete/5
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

        // POST: Dashboard/Orders/Delete/5
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
