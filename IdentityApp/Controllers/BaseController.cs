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

        //User.Identity.Name DB den degil Cookie'den geliyor.
        protected AppUser CurrentUser => userManager.FindByNameAsync(User.Identity.Name).Result;

        //ctor D.I
        public BaseController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
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
