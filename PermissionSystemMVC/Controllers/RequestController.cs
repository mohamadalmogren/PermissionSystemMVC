using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PermissionSystemMVC.Models;
using PermissionSystemMVC.Models.ViewModels;
using PermissionSystemMVC.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace PermissionSystemMVC.Controllers
{
    [Authorize]
    public class RequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public RequestController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // Get Request for manager
            if (User.IsInRole(RolesName.Manager))
            {
                var user = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
                var requests = await _context.Request
                    .Include(r => r.CreatedBy)
                    .Where(r => r.CreatedBy.DepartmentId == user.DepartmentId)
                    .ToListAsync();
                return View(requests);

            }
            else if (User.IsInRole(RolesName.Employee))
            {
                // Get Reuests for Employee
                var requests = await _context.Request.Where(r => r.CreatedById == User.GetUserId()).ToListAsync();

                return View(requests);
            }
            else
            {
                var requests = await _context.Request.ToListAsync();
                return View(requests);
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(ViewRequest request)
        {
            if (ModelState.IsValid)
            {
                var requestNew = new Request
                {
                    PrmisssionType= request.PrmisssionType,
                    DatePrmission= request.DatePrmission,
                    FromTime = request.FromTime,
                    ToTime = request.ToTime,
                    CreatedById = User.GetUserId(),
                    Status = "New"
                };
                _context.Request.Add(requestNew);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(request);
        }

    }
}
