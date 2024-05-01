using Microsoft.AspNetCore.Mvc;

namespace CarWorkshopManager.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (TempData.ContainsKey("Status"))
            {
                if (TempData["Status"].ToString() == "1")
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "UserAuth");
                }
            }
            else
            {
                return RedirectToAction("Index", "UserAuth");
            }

        }
    }
}
