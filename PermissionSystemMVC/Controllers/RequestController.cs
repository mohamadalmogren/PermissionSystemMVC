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
                    PrmisssionType = request.PrmisssionType,
                    DatePrmission = request.DatePrmission,
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

        public async Task<IActionResult> Approve(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _context.Request.FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        [HttpPost, ActionName("Approve")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveConfirmed(int id)
        {
            var request = await _context.Request.FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            if (string.Equals(request.Status, "New") == false)
            {
                return BadRequest();
            }

            request.Status = "Approve";

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Reject(int? id)
        {
            if(id== null)
            {
                return NotFound();
            }
            var request = await _context.Request.FirstOrDefaultAsync(r => r.Id == id);
            if(request== null)
            {
                return NotFound();
            }
            return View(request);
        }

        [HttpPost, ActionName("Reject")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectConfirmed(int id)
        {
            var request = await _context.Request.FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
            {
                return NotFound();
            }
            request.Status = "Reject";
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}
