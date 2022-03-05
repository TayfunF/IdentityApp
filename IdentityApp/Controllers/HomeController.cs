using IdentityApp.Models;
using IdentityApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public UserManager<AppUser> userManager { get; }

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager)
        {
            _logger = logger;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        

        //--------------------------------------------------------------------
        //ÜYE OL SAYFASI GET-POST METODUM
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserVM userVM)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = new AppUser();
                appUser.UserName = userVM.UserName;
                appUser.Email = userVM.Email;
                //appUser.PhoneNumber = userVM.PhoneNumber;
                if (userManager.Users.Any(x => x.PhoneNumber == userVM.PhoneNumber) && userVM.PhoneNumber !=null)
                {
                    ModelState.AddModelError("", "Bu telefon numarası zaten kayıtlı.");
                }

                //Şifreyi string olarak tutuyorum. Bunu Hash lemem lazım. O yüzden direk appUser ile almadım.
                IdentityResult result = await userManager.CreateAsync(appUser, userVM.Password);
                //Başarıyla kayıt olduysa Login ekranına gönder
                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Home");
                }
                //Eğer kullanıcının girdiği değerlerde bir hata varsa bu hatayı göster
                else
                {
                    //Türkçeleştirmesi CustomValidation klasörü ile yapılacak
                    foreach (IdentityError item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }

            return View(userVM);
        }

        //--------------------------------------------------------------------
        //GİRİŞ YAP SAYFASI GET-POST METODUM
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }















        //MVC PROJE İLE GELDİ BURASI ÖNEMLİ DEĞİL

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
