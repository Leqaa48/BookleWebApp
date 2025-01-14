using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookleWebApp.Areas.Dashboard.Controllers
{
    public class HomeController : Controller
    {
        [Area("Dashboard")]
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return RedirectToAction("Index" , "Orders" , new { area ="Dashboard"});
        }
    }
}
