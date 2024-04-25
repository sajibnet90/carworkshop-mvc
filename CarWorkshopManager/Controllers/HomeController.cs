using Microsoft.AspNetCore.Mvc;

namespace CarWorkshopManager.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
