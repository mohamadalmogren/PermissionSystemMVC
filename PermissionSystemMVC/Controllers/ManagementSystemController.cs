using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PermissionSystemMVC.Models;
using PermissionSystemMVC.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace PermissionSystemMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManagementSystemController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ManagementSystemController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> ManageUsers()
        {
            //var users = _context.Users.Include(u => u.Department).AsQueryable();
            //var roles = _context.Roles.Select(x => x.Name);
            var modle = from users in _context.Users
                        join userRole in _context.UserRoles on users.Id equals userRole.UserId
                        join role in _context.Roles on userRole.RoleId equals role.Id
                        join dep in _context.Departments on users.DepartmentId equals dep.Id
                        select new UserListViewModel
                        {
                            Username = users.UserName,
                            Email = users.Email,
                            Name = users.Name,
                            Department = dep.Name,
                            Role = role.Name
                        };

            return View(modle);
        }

        public IActionResult CreateUsers()
        {
            ViewData["DepList"] = new SelectList(_context.Departments, "Id", "Name");
            ViewData["RoleList"] = new SelectList(_context.Roles, "Name", "Name");

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUsers(ViewCreateUsersModels model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new()
                {
                    UserName = model.Username,
                    Email = model.Email,
                    Name = model.Name,
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    DepartmentId = model.DepartmentId
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(user, model.Role).Wait();
                    return RedirectToAction(nameof(ManageUsers));

                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

            }

            ViewData["DepList"] = new SelectList(_context.Departments, "Id", "Name");
            ViewData["RoleList"] = new SelectList(_context.Roles, "Name", "Name");
            return View(model);
        }

        public async Task<IActionResult> ManageDepartments()
        {
            var departments = await _context.Departments.ToListAsync();
            return View(departments);
        }

        public IActionResult CreateDepartment()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDepartment(ViewCreateDepartmentModel department)
        {
            if (ModelState.IsValid)
            {
                var departmentNew = new Department
                { Name = department.Name };
                _context.Departments.Add(departmentNew);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Department));
            }
            else
            {
                return BadRequest();
            }
        }


    }
}
