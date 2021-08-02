using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PermissionSystemMVC.Models
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {


            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);


            //builder.Entity<Department>()
            //    .HasOne(b => b.Manager)
            //    .WithOne(i => i.Department)
            //    .HasForeignKey<Manager>(b => b.DepartmentForeignKeyId);

            builder.Entity<Request>()
                .Property(s => s.PrmisssionType)
                .HasConversion<string>();

            builder.Entity<Request>()
                .Property(s => s.Status)
                .HasConversion<string>();
        }


        public DbSet<Request> Request { get; set; }
        public DbSet<Department> Departments { get; set; }
    }
}
