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

        //--------------------------------------------------------------------
        //SIFRE YENILEME SAYFASI GET-POST METODUM
        [HttpGet]
        public IActionResult PasswordChange()
        {
            return View();
        }

        [HttpPost]
        public IActionResult PasswordChange(PasswordChangeVM passwordChangeVM)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = userManager.FindByNameAsync(User.Identity.Name).Result; //Identity.Name DB den degil Cookie'den geliyor.

                bool exist = userManager.CheckPasswordAsync(appUser, passwordChangeVM.PasswordOld).Result; //Eski sifresi var mi kontrol et

                if (exist)
                {
                    IdentityResult result = userManager.ChangePasswordAsync(appUser, passwordChangeVM.PasswordOld, passwordChangeVM.PasswordNew).Result;
                    if (result.Succeeded)
                    {
                        //userManager.UpdateSecurityStampAsync(appUser); // Bunu yazinca 30 dakika sonra kullanici otomatik cikis yapacak. 
                        //Usttekini yapmak yerine => kullaniciya backend tarafinda cikis yaptirip giris yaptiricam
                        signInManager.SignOutAsync();
                        signInManager.PasswordSignInAsync(appUser, passwordChangeVM.PasswordNew, true, false);

                        ViewBag.status = "success";
                    }
                    else
                    {
                        //Hatalar varsa onlari gostersin diye
                        foreach (var item in result.Errors)
                        {
                            ModelState.AddModelError("", item.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Eski Şifre yanlış !");
                }
            }
            return View(passwordChangeVM);
        }
    }
}
