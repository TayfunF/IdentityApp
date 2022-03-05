using IdentityApp.CustomValidation;
using IdentityApp.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
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
            services.AddControllersWithViews();
            //---------------------------------------------------------------------------------------------------
            //BURANIN ARASINA EKLEME YAPIYORUM
            services.AddMvc();
            services.AddDbContext<AppIdentityDbContext>(
            options => options.UseSqlServer("name=ConnectionStrings:DefaultConnection").UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequiredLength = 4; //Þifre en az 4 karakter olabilir.
                options.Password.RequireNonAlphanumeric = false; //Þifrede * . ? gibi þeyler giremesin dedim.
                options.Password.RequireLowercase = false; //Þifrede Küçük karakter girme zorunluluðunu kaldýrdým
                options.Password.RequireUppercase = false; //Þifrede Büyük karakter girme zorunluluðunu kaldýrdým.
                options.Password.RequireDigit = false; //0 dan 9 a kadar þifre giremesin dedim.

                options.User.RequireUniqueEmail = true; //E-posta adresi uniq olmalý dedim.

                //https://docs.microsoft.com/tr-tr/dotnet/api/microsoft.aspnetcore.identity.useroptions.allowedusernamecharacters?view=aspnetcore-6.0
                //UserName sadece þu karakterlerden oluþabilir dedim.
                options.User.AllowedUserNameCharacters = "abcçdefgðhýijklmnoöpqrsþtuüvwxyzABCÇDEFGÐHIÝJKLMNOÖPQRSÞTUÜVWXYZ0123456789-._";

            }).AddPasswordValidator<CustomPasswordValidator>()
            .AddUserValidator<CustomUserValidator>()
            .AddEntityFrameworkStores<AppIdentityDbContext>();
            //BURANIN ARASINA EKLEME YAPIYORUM
            //---------------------------------------------------------------------------------------------------
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            //---------------------------------------------------------------------------------------------------
            //BURANIN ARASINA EKLEME YAPIYORUM
            app.UseStatusCodePages();
            app.UseAuthentication();
            //BURANIN ARASINA EKLEME YAPIYORUM
            //---------------------------------------------------------------------------------------------------

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
