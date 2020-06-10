using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreIdentitySample.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CoreIdentitySample
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                #region Seed Sehirler
                var db = services.GetRequiredService<ApplicationDbContext>();

                if (!db.Sehirler.Any())
                {
                    db.Sehirler.Add(new Sehir { Id = 6, SehirAd = "Ankara" });
                    db.Sehirler.Add(new Sehir { Id = 35, SehirAd = "Izmir" });
                    db.SaveChanges();
                }
                #endregion

                #region Seed Roles
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                await roleManager.CreateAsync(new IdentityRole { Name = "admin"});
                #endregion

                #region Seed Users
                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

                var adminUser = new IdentityUser { UserName = "a@a.com", Email = "a@a.com", EmailConfirmed = true };
                await userManager.CreateAsync(adminUser, "Ankara1.");

                adminUser = await userManager.FindByNameAsync("a@a.com");
                await userManager.AddToRoleAsync(adminUser, "admin");

                var sampleUser = new IdentityUser { UserName = "s@s.com", Email = "s@s.com", EmailConfirmed = true };
                await userManager.CreateAsync(sampleUser, "Ankara1.");
                #endregion
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
