using IdentityApp.Models;
using IdentityApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace IdentityApp.Controllers
{
    public class AdminController : BaseController
    {

        public AdminController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager) : base(userManager, null, roleManager)
        {
        }
        public IActionResult Index()
        {
            return View();
        }

        //--------------------------------------------------------------------
        //UYELER SAYFASI GET METODUM
        public IActionResult Users()
        {
            return View(userManager.Users.ToList());
        }

        //--------------------------------------------------------------------
        //ROLLER SAYFASI GET METODUM
        public IActionResult Roles()
        {
            return View(roleManager.Roles.ToList());
        }

        //--------------------------------------------------------------------
        //UYE ROLU EKLEME SAYFASI GET-POST METODUM
        [HttpGet]
        public IActionResult RoleCreate()
        {
            return View();
        }
        [HttpPost]
        public IActionResult RoleCreate(RoleVM roleVM)
        {
            AppRole role = new AppRole();
            role.Name = roleVM.Name;
            IdentityResult result = roleManager.CreateAsync(role).Result;

            if (result.Succeeded)
            {
                return RedirectToAction("Roles", "Admin");
            }
            else
            {
                AddModelError(result);
            }
            return View(roleVM);
        }
    }
}
