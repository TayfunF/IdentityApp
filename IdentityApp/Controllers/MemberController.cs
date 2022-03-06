using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mapster;
using IdentityApp.ViewModels;

namespace IdentityApp.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        public UserManager<AppUser> userManager { get; }
        public SignInManager<AppUser> signInManager { get; }

        public MemberController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public IActionResult Index()
        {
            AppUser appUser = userManager.FindByNameAsync(User.Identity.Name).Result;
            UserVM userVM = appUser.Adapt<UserVM>(); //Mapster Kutuphane Kullanimi
            return View(userVM);
        }
    }
}
