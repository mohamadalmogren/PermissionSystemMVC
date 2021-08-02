using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace PermissionSystemMVC.Models
{
    public static class SeedData
    {

        public static void Initialize(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            using var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
            CreateDepartments(context);
            context.SaveChanges();

            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        private static void SeedUsers(UserManager<AppUser> userManager)
        {
            if (userManager.FindByEmailAsync("Admin@localhost").Result == null)
            {
                AppUser user = new()
                {
                    UserName = "Admin",
                    Email = "Admin@localhost",
                    Name = "Admin",
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    DepartmentId = 1
                };

                var result = userManager.CreateAsync(user, "Zaq!1234").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, RolesName.Admin).Wait();
                }
            }


            if (userManager.FindByEmailAsync("TestEMPLOYEE@localhost").Result == null)
            {
                AppUser user = new()
                {
                    UserName = "TestEmp",
                    Email = "TestEMPLOYEE@localhost",
                    Name = "Mohammed",
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    DepartmentId = 2

                };

                IdentityResult result = userManager.CreateAsync(user, "Zaq!1234").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, RolesName.Employee).Wait();
                }
            }

            if (userManager.FindByEmailAsync("TestMANAGER@localhost").Result == null)
            {
                AppUser user = new()
                {
                    UserName = "TestManager",
                    Email = "Manager@localhost",
                    Name = "Abdullah",
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    DepartmentId = 2
                };


                IdentityResult result = userManager.CreateAsync(user, "Zaq!1234").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, RolesName.Manager).Wait();
                }
            }
        }

        private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync(RolesName.Manager).Result)
            {
                var result = roleManager.CreateAsync(new IdentityRole { Name = RolesName.Manager }).Result;
                if (result.Succeeded == false)
                {
                    // log error
                }
            }


            if (!roleManager.RoleExistsAsync(RolesName.Employee).Result)
            {
                IdentityRole role = new()
                {
                    Name = RolesName.Employee
                };

                var result = roleManager.CreateAsync(role).Result;
                if (result.Succeeded == false)
                {
                    // log error
                }
            }

            if (!roleManager.RoleExistsAsync(RolesName.Admin).Result)
            {
                IdentityRole role = new()
                {
                    Name = RolesName.Admin
                };

                var result = roleManager.CreateAsync(role).Result;
                if (result.Succeeded == false)
                {
                    // log error
                }
            }
        }

        private static void CreateDepartments(ApplicationDbContext context)
        {
            if (context.Departments.Any())
            {
                return;
            }
            context.Departments.AddRange(
                new Department { Name = "Admin" },
                new Department { Name = "Development" },
                new Department { Name = "Business Analysis" }
            );
        }

       
        
    }
}
