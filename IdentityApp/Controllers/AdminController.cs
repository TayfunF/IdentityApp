using IdentityApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace IdentityApp.Controllers
{
    public class AdminController : BaseController
    {

        public AdminController(UserManager<AppUser> userManager,RoleManager<AppRole> roleManager):base(userManager,null,roleManager)
        {
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Users()
        {
            return View(userManager.Users.ToList());
        }
    }
}
