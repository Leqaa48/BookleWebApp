using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookleWebApp.Data;
using BookleWebApp.Models;
using System.Security.Claims;
using BookleWebApp.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace BookleWebApp.Controllers
{
    
    public class BooksController : Controller
    {
        private UserManager<ApplicationUser> userManager;
        private SignInManager<ApplicationUser> signInManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        public BooksController(UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> _signInManager,
           RoleManager<IdentityRole> roleManager,
           AppDbContext context)
        {
            this.userManager = userManager;
            signInManager = _signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        // GET: Dashboard/Books
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Books.Include(b => b.Category).Include(b => b.Publisher);
            return View(await appDbContext.ToListAsync());
        }
        public async Task<IActionResult> Shop(string? query)
        {
            // Start with a queryable collection of books including related data
            IQueryable<Book> appDbContext = _context.Books
                .Include(b => b.Category)
                .Include(b => b.Publisher);

            // Apply filtering if a search query is provided
            if (!string.IsNullOrEmpty(query))
            {
                appDbContext = appDbContext.Where(b => b.Title.Contains(query) || b.Author.Contains(query));
            }

            // Retrieve categories for sidebar
            ViewBag.Categories = _context.Categories.ToList();

            // Pass the search query to the view for the input field
            ViewBag.SearchQuery = query;

            // Execute the query and return the view
            return View(await appDbContext.ToListAsync());
        }

        // GET: Dashboard/Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            
            return View(book);
        }
        public async Task<IActionResult> ShopList()
        {
            var books = _context.Books.ToList();
            return View(books);
        }
        public async Task<IActionResult> LoginFirst(int? id)
        {
            ViewBag.GetBookId = id;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoginFirst(int id, LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to sign in the user.
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    // Redirect to the "BookDetails" action, passing the id as a route value.
                    return RedirectToAction("BookDetails", "Books", new { id = id });
                }

                // Add an error message if login fails.
                ModelState.AddModelError("", "Invalid user or password");
            }

            // Return the view with the model if login fails or ModelState is invalid.
            return View(model);
        }

        // GET: Dashboard/Books/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Name");
            return View();
        }

        // POST: Dashboard/Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            if (ModelState.IsValid)
            {
                // Handle image upload
                if (book.ImageFile != null && book.ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(book.ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await book.ImageFile.CopyToAsync(fileStream);
                    }

                    book.BookImage = $"/images/{uniqueFileName}";
                }

                // Set the PublisherId to the logged-in user's ID
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    ModelState.AddModelError("", "Invalid user ID.");
                    ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", book.CategoryId);
                    return View(book);
                }

                book.PublisherId = userId; // Assign the logged-in user's ID

                // Add the book to the database
                _context.Add(book);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Publishers");
            }

                // If the model state is invalid, repopulate the ViewData and return to the view
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", book.CategoryId);
            return View(book);
        }
        // GET: Dashboard/Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", book.CategoryId);
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Name", book.PublisherId);
            return View(book);
        }

        // POST: Dashboard/Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (book.ImageFile != null && book.ImageFile.Length > 0)
                    {
                        // Define the path to save the image
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                        var fileName = Path.GetFileName(book.ImageFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        // Ensure the uploads folder exists
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Save the file to the server
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await book.ImageFile.CopyToAsync(fileStream);
                        }

                        // Update the image path in the model
                        book.BookImage = $"/images/{fileName}";
                    }
                    else
                    {
                        // If no new file is uploaded, retain the existing image path
                        var existingCategory = await _context.Books.FindAsync(id);
                        book.BookImage = existingCategory.BookImage;
                    }
                    // Set the PublisherId to the logged-in user's ID
                    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(userId))
                    {
                        ModelState.AddModelError("", "Invalid user ID.");
                        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", book.CategoryId);
                        return View(book);
                    }

                    book.PublisherId = userId; // Assign the logged-in user's ID
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
    return RedirectToAction("Index", "Publishers");
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", book.CategoryId);
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Name", book.PublisherId);
            return View(book);
        }

        // GET: Dashboard/Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Dashboard/Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Publishers");
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }

        public async Task<IActionResult> BooksInCategory(int id)
        {
            // Retrieve the category from the database by its id
            var cat = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            // Check if the category was found
            if (cat == null)
            {
                return NotFound(); // If category doesn't exist, return 404
            }

            // Retrieve books in the specific category
            var booksInCategory = await _context.Books
                                                 .Where(b => b.CategoryId == cat.Id)
                                                 .ToListAsync();
            ViewBag.CategoryName = cat.Name;
            // Return the books to the view
            return View(booksInCategory);
        }
        [Route("BookDetails/{Id?}")]
        public async Task<IActionResult> BookDetails(int? id)
        {
            if (id == null)
            {
               
                    return NotFound();
                
                    
            }

            var book = await _context.Books
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            ViewBag.BooksInSameCategory = _context.Books.Where(c=>c.CategoryId == book.CategoryId).ToList();
            return View(book);
        }
    }
}
