using IdentityApp.CustomValidation;
using IdentityApp.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //---------------------------------------------------------------------------------------------------
            //BURANIN ARASINA EKLEME YAPIYORUM
            //DB Service
            services.AddDbContext<AppIdentityDbContext>(options => options.UseSqlServer("name=ConnectionStrings:DefaultConnection").UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
            //Identity Service
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.User.RequireUniqueEmail = true; //E-posta adresi uniq olmalý dedim.
                //https://docs.microsoft.com/tr-tr/dotnet/api/microsoft.aspnetcore.identity.useroptions.allowedusernamecharacters?view=aspnetcore-6.0
                //UserName sadece þu karakterlerden oluþabilir dedim.
                options.User.AllowedUserNameCharacters = "abcçdefgðhýijklmnoçpqrsþtuüvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";

                options.Password.RequiredLength = 4; //Sifre en az 4 karakter olabilir.
                options.Password.RequireNonAlphanumeric = false; //Sifrede * . ? gibi seyler giremesin dedim.
                options.Password.RequireLowercase = false; //Sifrede Kucuk karakter girme zorunlulugunu kaldirdim.
                options.Password.RequireUppercase = false; //Sifrede Buyuk karakter girme zorunlulugunu kaldirdim.
                options.Password.RequireDigit = false; //0 dan 9 a kadar sifre giremesin dedim.
            }).AddPasswordValidator<CustomPasswordValidator>()
            .AddUserValidator<CustomUserValidator>()
            .AddErrorDescriber<CustomIdentityErorDescriber>()
            .AddEntityFrameworkStores<AppIdentityDbContext>()
            .AddDefaultTokenProviders();

            //Cookiebuilder Service
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie = new CookieBuilder()
                {
                    Name = "MyBlog", //Cookie Adi
                    HttpOnly = false, //Http olursa kabul etme
                    SameSite = SameSiteMode.Lax,
                    SecurePolicy = CookieSecurePolicy.SameAsRequest //Browsera istek Http ise Http ile al , Https ise Https ile al.
                };
                options.LoginPath = new PathString("/Home/Login"); //Kullanici uye olmadan üyelerin erisebildigi yere tiklarsa Login'e yonlendir.
                options.LogoutPath = new PathString("/Member/Logout"); //Cikis Yap. _MemberLayout.cshtl icinde cikisyap linkinde tanimladim.
                options.SlidingExpiration = true; //Kullanici 30 gun sonra siteme istek yaparsa 60 gun daha oturumunu sakla.
                options.ExpireTimeSpan = System.TimeSpan.FromDays(60);
                options.AccessDeniedPath = new PathString("/Member/AccessDenied"); //Erisim Reddedildi Sayfasi
            });
            //BURANIN ARASINA EKLEME YAPIYORUM
            //---------------------------------------------------------------------------------------------------
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseDeveloperExceptionPage();
            app.UseStatusCodePages();
            app.UseStaticFiles();
            
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            //app.UseExceptionHandler("/Home/Error");

            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //app.UseHsts();

            //app.UseHttpsRedirection();
        }
    }
}
