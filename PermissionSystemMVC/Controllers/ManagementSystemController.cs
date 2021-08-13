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
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

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
            var usersList = new List<UserListViewModel>();
            var users = _context.Users.Include(u => u.Department).ToList();

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var userObj = new UserListViewModel
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    Name = user.Name,
                    Departmentname = user.Department.Name,
                    Roles = string.Join(',', userRoles)
                };

                usersList.Add(userObj);
            }

            return View(usersList);
        }
        [HttpGet]
        public async Task<ActionResult> GetAllUsers()
        {
            var usersList = new List<UserListViewModel>();
            var users = _context.Users.Include(u => u.Department).ToList();

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var userObj = new UserListViewModel
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    Name = user.Name,
                    Departmentname = user.Department.Name,
                    Roles = string.Join(',', userRoles)
                };

                usersList.Add(userObj);
            }
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() },
            };

            return Json(new { data = usersList }, options);
        }

        public IActionResult CreateUsers()
        {
            ViewData["DepList"] = new SelectList(_context.Departments, "Id", "Name");
            ViewData["RoleList"] = _context.Roles.Select(r => r.Name).ToList();

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

                var rolesList = model.Roles;

                if (result.Succeeded)
                {
                    foreach (var role in rolesList)
                    {
                        _userManager.AddToRoleAsync(user, role).Wait();

                    }
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
        public async Task<IActionResult> EditUser(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Username = user.UserName,
                Email = user.Email,
                Roles = userRoles,
                DepartmentId = user.DepartmentId
            };

            ViewData["DepList"] = new SelectList(_context.Departments, "Id", "Name");
            ViewData["RoleList"] = _context.Roles.Select(r => r.Name).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel userToEdit)
        {
            var rolesList = _context.Roles.Select(r => r.Name).ToList();
            var userInDb = await _context.Users.FirstOrDefaultAsync(x => x.Id == userToEdit.Id);

            if (userInDb == null)
            {
                return NotFound();
            }

            var userRolesInDb = await _userManager.GetRolesAsync(userInDb);

            if (ModelState.IsValid)
            {
                try
                {
                    //user.Email = model.Email; To Do
                    userInDb.Name = userToEdit.Name;
                    userInDb.DepartmentId = userToEdit.DepartmentId;

                    foreach (var role in rolesList)
                    {
                        if (userRolesInDb.Contains(role) && userToEdit.Roles.Contains(role))
                        {
                            continue;
                        }
                        if (userRolesInDb.Contains(role) && userToEdit.Roles.Contains(role) == false)
                        {
                            var result = await _userManager.RemoveFromRoleAsync(userInDb, role);
                            if (result.Succeeded == false)
                            {
                                foreach (var error in result.Errors)
                                {
                                    ModelState.AddModelError(string.Empty, error.Description);
                                }
                            }
                        }
                        if (userRolesInDb.Contains(role) == false && userToEdit.Roles.Contains(role))
                        {
                            var result = await _userManager.AddToRoleAsync(userInDb, role);
                            if (result.Succeeded == false)
                            {
                                foreach (var error in result.Errors)
                                {
                                    ModelState.AddModelError(string.Empty, error.Description);
                                }
                            }
                        }
                    }

                    if (ModelState.ErrorCount > 0 == false)
                    {
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(ManageUsers));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Users.Any(y => y.Id == userToEdit.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewData["DepList"] = new SelectList(_context.Departments, "Id", "Name");
            ViewData["RoleList"] = _context.Roles.Select(r => r.Name).ToList();
            return View(userToEdit);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Json(new { resulte = true, msg = "User Deleted!" });
        }


        public async Task<IActionResult> ManageDepartments()
        {
            var departments = await _context.Departments.ToListAsync();
            return View(departments);
        }

        [HttpGet]
        public async Task<ActionResult> GetAllDepartments()
        {
            var retrunList = new List<ListDepartmentViewModel>();

            var departments = await _context.Departments.ToListAsync();

            foreach (var item in departments)
            {
                retrunList.Add(new ListDepartmentViewModel
                {
                    Id = item.Id.ToString(),
                    Name = item.Name
                });
            }
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return Json(new { data = retrunList }, options);
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
                return RedirectToAction(nameof(ManageDepartments));
            }
            else
            {
                return BadRequest();
            }
        }
        public async Task<IActionResult> GetEditDepartment(int id)
        {

            var department = await _context.Departments.FindAsync(id);

            if (department == null)
            {
                return NotFound();
            }
            return Json(new { resulte = department, msg = "edited Department!" });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDepartment(Department department)
        {
            var dept = await _context.Departments.FindAsync(department.Id);

            if (dept == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    dept.Name = department.Name;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Departments.Any(i => i.Id == department.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ManageDepartments));
            }
            return View(department);
        }

        
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
            return Json(new { resulte = true, msg = "Department Deleted!" });

        }
    }
}
