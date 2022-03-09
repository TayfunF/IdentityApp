using IdentityApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IdentityApp.Controllers
{
    public class BaseController : Controller
    {
        //COK FAZLA TEKRAR EDEN KODLARI BURADA TOPLUYORUM

        protected UserManager<AppUser> userManager { get; }
        protected SignInManager<AppUser> signInManager { get; }
        protected RoleManager<AppRole> roleManager { get; }

        //User.Identity.Name , DB den degil Cookie'den geliyor.(Yani Login olan kullanicinin adi)
        protected AppUser CurrentUser => userManager.FindByNameAsync(User.Identity.Name).Result;

        //ctor D.I roleManager diger Controllerlarda patlamasın diye null verdim
        public BaseController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager=null)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        //Hata İcin Kullanidigim metod. Turkcelestirmesi CustomValidation Klasorunde
        public void AddModelError(IdentityResult result)
        {
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError("", item.Description);
            }
        }
    }
}
