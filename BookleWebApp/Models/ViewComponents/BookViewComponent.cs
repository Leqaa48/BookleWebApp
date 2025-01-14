using BookleWebApp.Data;
using Microsoft.AspNetCore.Mvc;

namespace BookleWebApp.Models.ViewComponents
{
    public class BookViewComponent :ViewComponent
    {
        private AppDbContext db;
        public BookViewComponent(AppDbContext _db)
        {
            db = _db;

        }
        public IViewComponentResult Invoke()
        {
            return View(db.Books.ToList());
        }
    }
}
