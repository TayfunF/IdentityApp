using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mapster;
using IdentityApp.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using IdentityApp.Enums;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace IdentityApp.Controllers
{
    [Authorize]
    public class MemberController : BaseController
    {
        public MemberController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) : base(userManager, signInManager)
        {
        }

        public IActionResult Index()
        {
            AppUser appUser = CurrentUser;
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
                AppUser appUser = CurrentUser;

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
                        AddModelError(result);
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
            AppUser appUser = CurrentUser;
            //Mapster
            UserVM userVM = appUser.Adapt<UserVM>();
            //Cinsiyeti Dropdown olarak almak için
            ViewBag.Gender = new SelectList(Enum.GetNames(typeof(Gender)));
            return View(userVM);
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserVM userVM, IFormFile userPicture)
        {
            //Bu metod icinde Sifre guncelleme islemi yaptirmadigim icin Vm den gelen Password propunu kaldirdim.
            ModelState.Remove("Password");
            //Cinsiyeti Dropdown olarak almak için
            ViewBag.Gender = new SelectList(Enum.GetNames(typeof(Gender)));

            if (ModelState.IsValid)
            {
                AppUser appUser = CurrentUser;

                //Bilgi Guncelleme Sayfasinda Kullanici resmini almak icin (Identity ile Alakasi Yok !!!)
                if (userPicture != null && userPicture.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(userPicture.FileName);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserImages", fileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await userPicture.CopyToAsync(stream);
                        appUser.Picture = "/UserImages/" + fileName;
                    }
                }

                appUser.UserName = userVM.UserName;
                appUser.PhoneNumber = userVM.PhoneNumber;
                appUser.Email = userVM.Email;
                appUser.City = userVM.City;
                appUser.BirthDay = userVM.BirthDay;
                appUser.Gender = (int)userVM.Gender; //Enum oldugu icin int donusumu yaptim

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
                    AddModelError(result);
                }

            }
            return View(userVM);
        }

        //--------------------------------------------------------------------
        //KULLANICI BILGILERI GUNCELLEME SAYFASI GET-POST METODUM
        //Starup.cs de tanimli oldugu icin void li yaptim.
        public void Logout()
        {
            signInManager.SignOutAsync();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        //--------------------------------------------------------------------
        //SADECE EDITOR LERIN GIRECEGI SAYFA
        [Authorize(Roles = "editor,admin")]
        public IActionResult Editor()
        {
            return View();
        }

        //--------------------------------------------------------------------
        //SADECE MANAGER LARIN GIRECEGI SAYFA
        [Authorize(Roles = "manager,admin")]
        public IActionResult Manager()
        {
            return View();
        }
    }
}
