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
        public SignInManager<AppUser> signInManager { get; }

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            this.userManager = userManager;
            this.signInManager = signInManager;
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
                if (userManager.Users.Any(x => x.PhoneNumber == userVM.PhoneNumber) && userVM.PhoneNumber != null)
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
        public IActionResult Login(string ReturnUrl)
        {
            TempData["ReturnUrl"] = ReturnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await userManager.FindByEmailAsync(loginVM.Email);

                if (user != null)
                {
                    await signInManager.SignOutAsync(); //Çıkış yaptır ilk
                    //Eğer kullanıcı giriş yaparken 'Beni Hatırla' yı işaretlediyse 'Startup.cs' ye gidicek ve kullanıcının bilgilerini 60 gün tutacak.
                    Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, false); //Giriş Yaptır
                    if (result.Succeeded)
                    {
                        if (TempData["ReturnUrl"] != null)
                        {
                            return Redirect(TempData["ReturnUrl"].ToString());
                        }

                        return RedirectToAction("Index", "Member");
                    }
                }
            }
            else
            {
                ModelState.AddModelError(nameof(LoginVM.Email), "Kullanıcı adı ya da şifre yanlış !");
            }
            return View(loginVM);
        }

        //--------------------------------------------------------------------
        //ŞİFREMİ UNUTTUM SAYFASI GET-POST METODUM
        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ResetPassword(PasswordResetVM passwordResetVM)
        {
            AppUser appUser = userManager.FindByEmailAsync(passwordResetVM.Email).Result;
            if (appUser != null)
            {
                //User bilgilerinden oluşan bir token oluşturuyor.
                string passwordResetToken = userManager.GeneratePasswordResetTokenAsync(appUser).Result;
                //(Github da Ders 22 de ) Eposta gönderme kısmını yapıcam
                string passwordResetLink = Url.Action("ResetPasswordConfirm", "Home", new
                {
                    userId = appUser.Id,
                    token = passwordResetToken
                }, HttpContext.Request.Scheme);

                Helper.PasswordResetHelper.PasswordResetSendEmail(passwordResetLink, appUser.Email);
                ViewBag.status = "success";
            }
            else
            {
                ModelState.AddModelError("", "Sistemde kayıtlı e-posta adresi bulunamadı !");
            }

            return View(passwordResetVM);
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
