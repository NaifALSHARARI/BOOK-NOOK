using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Privacy()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
