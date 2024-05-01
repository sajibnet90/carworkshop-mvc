using CarWorkshopManager.Data;
using CarWorkshopManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarWorkshopManager.Controllers
{
    public class UserAuthController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UserAuthController(IConfiguration configuration)
        {
            var factory = new ApplicationDbContextFactory(configuration);
            _db = factory.CreateDbContext();

        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(UserAuth model)
        {
            if(model.Username == "")
            {
                ViewBag.ErrorMessage = "Username Required";
                return RedirectToAction("Index","UserAuth");
            }
            if(model.Password == "")
            {
                ViewBag.ErrorMessage = "Password Required";
                return RedirectToAction("Index", "UserAuth");
            }

            var user = _db.userAuths.FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);
            if (user == null)
            {
                ViewBag.ErrorMessage = "Invalid username or password";
                return RedirectToAction("Index", "UserAuth");
            }

            // Redirect the user to the dashboard or another page
            TempData["Status"] = "1";
            return RedirectToAction("Index", "Home");
        }
    }
}
