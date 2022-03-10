using IdentityApp.Models;
using IdentityApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityApp.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) : base(userManager, signInManager)
        {

        }

        //Eğer kullanıcı Login işlemi yaptıysa Member/Index sayfasına gitsin dedim.
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
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

        //--------------------------------------------------------------------
        //FACEBOOK ILE GIRIS SAYFASI METODUM
        //delevoper.facebook sitesinden key,value aldim
        //Facebook ile giris yaptiktan sonra ben kullaniciyi erismeye calistigi sayfaya gondericem. Burdaki ReturnUrl anlami bu
        // Ornegin => Kullanici uyelerin erisebilecegi sayfaya tikladi ve sonrasinda Login ekranina geldi. (Uye olmadigi icin)
        public IActionResult FacebookLogin(string ReturnUrl)
        {
            //Kullanicinin facebooktaki islemleri bittikten sonra sayfaya geri gelecegi url yi belirtiyorum.
            string RedirectUrl = Url.Action("ExternalResponse", "Home", new { ReturnUrl = ReturnUrl });
            //Facebook ile girisle ilgili proplari hazirlama kodlari
            var properties = signInManager.ConfigureExternalAuthenticationProperties("Facebook", RedirectUrl);
            //ChanllangeResult icerisine ne alirsa , kullaniciyi oraya yonlendirir.
            return new ChallengeResult("Facebook", properties);
        }
        //FACEBOOK GOOGLE MICROSOFT ICIN BU ACTIONMETODUM ORTAK OLACAK
        //YANI => NEYLE GIRIS YAPTIRMAK ISTERSEM ilgili kod blogu icinde ExternalResponse metoduna gonderim yapmam yeterli
        //Eger kullanici yonlendirme ile gelmeden , direkt olarak login sayfasina geldiyse onu anasayfaya yonlendirme icin "/" kullandim.
        public async Task<IActionResult> ExternalResponse(string ReturnUrl = "/")
        {
            //Bana kullanicinin Login oldugu ile ilgili bazi bilgiler veriyor.
            ExternalLoginInfo info = await signInManager.GetExternalLoginInfoAsync();
            //Kullanici facebook ile login sayfasina gitti ama bilgilerini girmediyse diye kontrol yapiyorum
            if (info == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                //2 Cesit SignInResult var. Namespace vermemin sebebi cakisman olmasin diye.
                //Eger db de kullanici daha once login olmussa login islemi gerceklesecek
                Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);
                //Eger db de bu degerler yoksa bunlari kaydetmem lazim.(Yani ilk kez facebook ile login yapiyorsa gibi dusuneblirim)
                if (result.Succeeded)
                {
                    return Redirect(ReturnUrl);
                }
                else
                {
                    AppUser user = new AppUser();
                    user.Email = info.Principal.FindFirst(ClaimTypes.Email).Value;
                    string ExternalUserId = info.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;

                    if (info.Principal.HasClaim(x => x.Type == ClaimTypes.Name))
                    {
                        string userName = info.Principal.FindFirst(ClaimTypes.Name).Value;
                        //userName den gelecek olan bosluklari - haline getir sonra kucuk harf yap sonuna da Guid ekle
                        userName = userName.Replace(' ', '-').ToLower() + ExternalUserId.Substring(0, 5).ToString();
                        user.UserName = userName;
                    }
                    else
                    {
                        user.UserName = info.Principal.FindFirst(ClaimTypes.Email).Value;
                    }

                    IdentityResult createResult = await userManager.CreateAsync(user);

                    if (createResult.Succeeded)
                    {
                        //dbo.AspNetUsersLogins tablomu doldurmam lazim
                        //Bunu yazmazsa IdentityApi bu kullanicinin facebooktan login oldugunu anlayamaz
                        IdentityResult loginResult = await userManager.AddLoginAsync(user, info);

                        if (loginResult.Succeeded)
                        {
                            //Alt satirdaki gibi yaparsak kullanicinin sosyal medya ile girip girmedigini anlayamam.(ama Kullansamda yanlislik olmaz)
                            //await signInManager.SignInAsync(user, true);
                            await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);

                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            AddModelError(loginResult);
                        }
                    }
                    else
                    {
                        AddModelError(createResult);
                    }
                    //Hatalarin ne oldugunu liste seklinde al ErrorIdentity Sayfasina model olarak aldim
                    List<string> errors = ModelState.Values.SelectMany(x => x.Errors).Select(y => y.ErrorMessage).ToList();

                    return View("ErrorIdentity", errors);
                }
            }
        }

        //ERROR SAYFASI METODUM
        public IActionResult ErrorIdentity()
        {
            return View();
        }

        //--------------------------------------------------------------------
        //GOOGLE ILE GIRIS SAYFASI METODUM
        //console.developers.google sitesinden key,value aldim
        public IActionResult GoogleLogin(string ReturnUrl)
        {
            string RedirectUrl = Url.Action("ExternalResponse", "Home", new { ReturnUrl = ReturnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties("Google", RedirectUrl);

            return new ChallengeResult("Google");
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
