using IdentityApp.Models;
using IdentityApp.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityApp.Controllers
{
    [Authorize(Roles ="admin")]
    public class AdminController : BaseController
    {
        public AdminController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager) : base(userManager, null, roleManager)
        {
        }
        public IActionResult Index()
        {
            return View();
        }

        //--------------------------------------------------------------------
        //UYELER SAYFASI GET METODUM
        public IActionResult Users()
        {
            return View(userManager.Users.ToList());
        }

        //--------------------------------------------------------------------
        //ROLLER SAYFASI GET METODUM
        public IActionResult Roles()
        {
            return View(roleManager.Roles.ToList());
        }

        //--------------------------------------------------------------------
        //ROL OLUSTURMA SAYFASI GET-POST METODUM
        [HttpGet]
        public IActionResult RoleCreate()
        {
            return View();
        }
        [HttpPost]
        public IActionResult RoleCreate(RoleVM roleVM)
        {
            AppRole role = new AppRole();
            role.Name = roleVM.Name;
            IdentityResult result = roleManager.CreateAsync(role).Result;

            if (result.Succeeded)
            {
                return RedirectToAction("Roles", "Admin");
            }
            else
            {
                AddModelError(result);
            }
            return View(roleVM);
        }

        //--------------------------------------------------------------------
        //ROL SILME SAYFASI POST METODUM
        [HttpPost]
        public async Task<IActionResult> RoleDelete(string id)
        {
            AppRole role = roleManager.FindByIdAsync(id).Result;

            if (role != null)
            {
                await roleManager.DeleteAsync(role);
            }
            return RedirectToAction("Roles", "Admin");
        }

        //--------------------------------------------------------------------
        //ROL GUNCELLEME SAYFASI GET-POST METODUM
        [HttpGet]
        public IActionResult RoleUpdate(string id)
        {
            AppRole role = roleManager.FindByIdAsync(id).Result;

            return View(role.Adapt<RoleVM>()); //Mapster AppRole-RoleVM eslestirdim
        }
        [HttpPost]
        public IActionResult RoleUpdate(RoleVM roleVM)
        {
            AppRole role = roleManager.FindByIdAsync(roleVM.Id).Result;

            if (role != null)
            {
                role.Name = roleVM.Name;

                IdentityResult result = roleManager.UpdateAsync(role).Result;
                if (result.Succeeded)
                {
                    return RedirectToAction("Roles", "Admin");
                }
                else
                {
                    AddModelError(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "Güncelleme işlemi başarısız");
            }
            return View(roleVM);
        }

        //--------------------------------------------------------------------
        //ROL ATAMA SAYFASI GET-POST METODUM
        [HttpGet]
        public IActionResult RoleAssign(string id)
        {
            //Post kisminda kullanmak icin aliyorum
            TempData["user_id"] = id;
            //Hangi id nin yanindaki rol ata adli butona tiklandiysa onu getirdim
            AppUser appUser = userManager.FindByIdAsync(id).Result;
            //View tarafinda kullaniciAdini gostermek icin yazdim
            ViewBag.userName = appUser.UserName;
            //AppRole veritabanimda kayitli olan rolleri cektim. DB de kac tane Rol varsa, //var roles = roleManager.Roles.ToList();
            IQueryable<AppRole> roles = roleManager.Roles;
            //Id sine tiklanan kullanici hangi rollere sahip liste olarak dondur ? //var userroles = userManager.GetRolesAsync(appUser);
            List<string> userRoles = userManager.GetRolesAsync(appUser).Result as List<string>;

            List<RoleAssignVM> listele = new List<RoleAssignVM>();

            foreach (var item in roles)
            {
                //Bu VM yi Hem checkbox lari gosterebilmek icin hem de checkbox in isaretli olup olmadigini bilmek icin kullandim
                RoleAssignVM r = new RoleAssignVM();
                r.RoleId = item.Id;
                r.RoleName = item.Name;
                if (userRoles.Contains(item.Name))
                {
                    r.Exist = true;
                }
                else
                {
                    r.Exist = false;
                }
                listele.Add(r);
            }
            return View(listele);
        }

        [HttpPost]
        public IActionResult RoleAssign(List<RoleAssignVM> roleAssignVM)
        {
            AppUser appUser = userManager.FindByIdAsync(TempData["user_id"].ToString()).Result;
            //Bu metod icinde Ayni Sayfada Hem atama hem de atama islemini kaldirma yapmis oluyoruz.
            foreach (var item in roleAssignVM)
            {
                if (item.Exist)
                {
                    userManager.AddToRoleAsync(appUser, item.RoleName).Wait(); //Async oldugu icin Wait() koydum
                }
                else
                {
                    userManager.RemoveFromRoleAsync(appUser, item.RoleName).Wait();
                }
            }
            return RedirectToAction("Users", "Admin");
        }

    }
}
