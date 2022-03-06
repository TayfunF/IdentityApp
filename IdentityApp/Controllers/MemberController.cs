using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mapster;
using IdentityApp.ViewModels;
using System.Threading.Tasks;

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
                        // Bunu yazinca 30 dakika sonra kullanici otomatik cikis yapacak. 
                        //kullaniciya backend tarafinda cikis yaptirip giris yaptiricam
                        userManager.UpdateSecurityStampAsync(appUser);
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

        //--------------------------------------------------------------------
        //KULLANICI BILGILERI GUNCELLEME SAYFASI GET-POST METODUM
        [HttpGet]
        public IActionResult UserEdit()
        {
            AppUser appUser = userManager.FindByNameAsync(User.Identity.Name).Result;

            UserVM userVM = appUser.Adapt<UserVM>(); //Mapster

            return View(userVM);
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserVM userVM)
        {
            ModelState.Remove("Password"); //Bu alanda Sifre guncellemedigim icin Vm den gelen Passwordu kaldir.

            if (ModelState.IsValid)
            {
                AppUser appUser = await userManager.FindByNameAsync(User.Identity.Name);
                appUser.UserName = userVM.UserName;
                appUser.PhoneNumber = userVM.PhoneNumber;
                appUser.Email = userVM.Email;

                //Kullanici bilgi guncellerken hatali girerse kontrol etmek icin
                IdentityResult result = await userManager.UpdateAsync(appUser);

                if (result.Succeeded)
                {
                    // Bunu yazinca 30 dakika sonra kullanici otomatik cikis yapacak. 
                    //kullaniciya backend tarafinda cikis yaptirip giris yaptiricam
                    await userManager.UpdateSecurityStampAsync(appUser);
                    await signInManager.SignOutAsync();
                    await signInManager.SignInAsync(appUser, true);


                    ViewBag.status = "success";
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }

            }
            return View(userVM);
        }


    }
}
