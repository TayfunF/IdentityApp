using IdentityApp.Models;
using IdentityApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityApp.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) : base(userManager, signInManager)
        {

        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated) //Eğer kullanıcı Login işlemi yaptıysa Member/Index sayfasına gitsin dedim.
            {
                return RedirectToAction("Index", "Member");
            }

            return View();
        }

        //--------------------------------------------------------------------
        //UYE OL SAYFASI GET-POST METODUM
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

                //Şifreyi string olarak tutuyorum. Bunu Hash lemem lazim. O yuzden direkt appUser ile almadim.
                IdentityResult result = await userManager.CreateAsync(appUser, userVM.Password);
                //Basariyla kayit olduysa eposta onayi icin beklesin Login ekranina gönder
                if (result.Succeeded)
                {
                    string confirmationToken = userManager.GenerateEmailConfirmationTokenAsync(appUser).Result;
                    string link = Url.Action("ConfirmEmail", "Home", new
                    {
                        userId = appUser.Id,
                        token = confirmationToken
                    }, protocol: HttpContext.Request.Scheme);
                    Helper.EmailConfirmationHelper.EmailConfirmationSendEmail(link, appUser.Email);

                    return RedirectToAction("Login", "Home");
                }
                //Eğer kullanıcının girdiği değerlerde bir hata varsa bu hatayı göster
                else
                {
                    AddModelError(result);
                }
            }

            return View(userVM);
        }

        //--------------------------------------------------------------------
        //GIRIS YAP SAYFASI GET-POST METODUM
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

                    //register sonrasi email onay durumu false ise kullaniciyi bilgilendiricem
                    if (userManager.IsEmailConfirmedAsync(user).Result == false)
                    {
                        ModelState.AddModelError("", "E-posta adresinizi henüz onaylamadınız. Lütfen e-posta adresinizin gelen kutusunu kontrol ediniz");
                        return View(loginVM);
                    }

                    await signInManager.SignOutAsync(); //Cikis yaptir sonra giris yapsin
                    //Eger kullanici giris yaparken 'Beni Hatirla' yi isaretlediyse 'Startup.cs' ye gidicek ve kullanicinin bilgilerini 60 gun tutacak.
                    Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, false); //Giriş Yaptır
                    if (result.Succeeded)
                    {
                        if (TempData["ReturnUrl"] != null)
                        {
                            return Redirect(TempData["ReturnUrl"].ToString());
                        }

                        return RedirectToAction("Index", "Member");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Kullanıcı adı ya da şifre yanlış !");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Kullanıcı kaydı bulunamadı ! !");
                }
            }
            return View(loginVM);
        }

        //--------------------------------------------------------------------
        //SIFREMI UNUTTUM SAYFASI GET-POST METODUM
        [HttpGet]
        public IActionResult ResetPassword()
        {
            TempData["durum"] = null;
            return View();
        }
        [HttpPost]
        public IActionResult ResetPassword(PasswordResetVM passwordResetVM)
        {
            if (TempData["durum"] == null)
            {
                AppUser appUser = userManager.FindByEmailAsync(passwordResetVM.Email).Result;
                if (appUser != null)
                {
                    //User bilgilerinden olusan bir token olusturuyor.
                    string passwordResetToken = userManager.GeneratePasswordResetTokenAsync(appUser).Result;
                    string passwordResetLink = Url.Action("ResetPasswordConfirm", "Home", new
                    {
                        userId = appUser.Id,
                        token = passwordResetToken
                    }, HttpContext.Request.Scheme);

                    Helper.PasswordResetHelper.PasswordResetSendEmail(passwordResetLink, appUser.Email);
                    ViewBag.status = "success";
                    TempData["durum"] = true.ToString();
                }
                else
                {
                    ModelState.AddModelError("", "Sistemde kayıtlı e-posta adresi bulunamadı !");
                }
                return View(passwordResetVM);
            }
            else
            {
                return RedirectToAction("ResetPassword");
            }
        }
        //--------------------------------------------------------------------
        //SIFRE YENILEME SAYFASI GET-POST METODUM
        [HttpGet]
        public IActionResult ResetPasswordConfirm(string userId, string token)
        {
            TempData["userId"] = userId;
            TempData["token"] = token;
            return View();
        }

        //Bind => Modelimden kullanmak istedigim proplarim gelsin. Diger proplar gelmesin.
        [HttpPost]
        public async Task<IActionResult> ResetPasswordConfirm([Bind("PasswordNew")] PasswordResetVM passwordResetVM)
        {
            string userId = TempData["userId"].ToString();
            string token = TempData["token"].ToString();
            AppUser appUser = await userManager.FindByIdAsync(userId);

            if (appUser != null)
            {
                IdentityResult result = await userManager.ResetPasswordAsync(appUser, token, passwordResetVM.PasswordNew);

                if (result.Succeeded)
                {
                    await userManager.UpdateSecurityStampAsync(appUser); //Bunu yazmazsak kullanici eski sifresi ile dolanmaya devam eder.
                    ViewBag.status = "success";
                }
                else
                {
                    AddModelError(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "Hata meydana geldi. Lütfen daha sonra tekrar deneyin !");
            }
            return View(passwordResetVM);
        }

        //--------------------------------------------------------------------
        //EMAIL ONAYLAMA SAYFASI GET-POST METODUM
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await userManager.FindByIdAsync(userId);

            IdentityResult result = await userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                ViewBag.status = "E-posta adresiniz onaylanmıştır. Giriş Yapabilirsiniz";
            }
            else
            {
                ViewBag.status = "Bir hata meydana geldi. Lütfen daha sonra tekrar deneyin";
            }
            return View();
        }















        //MVC PROJE ILE GELDI BURASI ONEMLI DEGIL

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
