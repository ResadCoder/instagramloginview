using Microsoft.AspNetCore.Mvc;
using WebApplication6.Context;
using WebApplication6.Models;
using WebApplication6.ViewModel;

namespace WebApplication6.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(LoginViewModel model)
        {
            // Check if the user typed something
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                ViewBag.Error = "Username and password are required";
                return View(model);
            }

            // Create a new User entity
            var user = new User
            {
                Username = model.Username,
                Password = model.Password // ⚠️ in production, hash this!
            };

            // Save to database
            _context.Users.Add(user);
            _context.SaveChanges();

            ViewBag.Message = "User saved successfully!";
            return View(new LoginViewModel());
        }
    }
}