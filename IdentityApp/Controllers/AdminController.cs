using IdentityApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace IdentityApp.Controllers
{
    public class AdminController : Controller
    {
        private UserManager<AppUser> _userManager { get; }

        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var um = _userManager.Users.ToList();
            return View(um);
        }
    }
}
