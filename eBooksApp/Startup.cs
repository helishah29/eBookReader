using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eBooksApp.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eBooksApp
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultUI(UIFramework.Bootstrap4)
              .AddEntityFrameworkStores<ApplicationDbContext>().AddSignInManager<SignInManager<IdentityUser>>().AddDefaultTokenProviders();
              

            // services.AddDefaultIdentity<IdentityUser>()
            //.AddDefaultUI(UIFramework.Bootstrap4)
            // .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddSession();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

           // services.AddAuthorization(options =>
            //{
             //   options.AddPolicy("OnlyAdminAccess", policy => policy.RequireRole("Admin"));
           // });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            //CreateRoles(serviceProvider).Wait();
        }

      //  private async Task CreateRoles(IServiceProvider serviceProvider)
       // {
            //initializing custom roles 
        //    var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
         //   var UserManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        //    string[] roleNames = { "Admin", "User" };
         //   IdentityResult roleResult;

         //   foreach (var roleName in roleNames)
           // {
            //    var roleExist = await RoleManager.RoleExistsAsync(roleName);
           //     if (!roleExist)
           //     {
                    //create the roles and seed them to the database: Question 1
            //        roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
          //      }
          //  }

          //  IdentityUser user = await UserManager.FindByEmailAsync("htpatel@syr.edu");

          //  if (user == null)
           // {
              //  user = new IdentityUser()
              //  {
             //       UserName = "htpatel@syr.edu",
             //       Email = "htpatel@syr.edu",
            //    };
            //    await UserManager.CreateAsync(user, "Test@123");
         //   }
          //  await UserManager.AddToRoleAsync(user, "Admin");


           // IdentityUser user1 = await UserManager.FindByEmailAsync("testUser@syr.edu");

           // if (user1 == null)
            //{
               // user1 = new IdentityUser()
              //  {
              //      UserName = "testUser@syr.edu",
               //     Email = "testUser@syr.edu",
             //   };
             //   await UserManager.CreateAsync(user1, "Test@123");
           // }
           // await UserManager.AddToRoleAsync(user1, "User");



        
    }
}
