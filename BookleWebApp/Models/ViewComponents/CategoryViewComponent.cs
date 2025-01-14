using BookleWebApp.Data;
using Microsoft.AspNetCore.Mvc;

namespace BookleWebApp.Models.ViewComponents
{
    public class CategoryViewComponent : ViewComponent
    {
        private AppDbContext db;
        public CategoryViewComponent(AppDbContext _db)
        {
            db = _db;

        }
        public IViewComponentResult Invoke()
        {
            return View(db.Categories.ToList());
        }
    }
}
