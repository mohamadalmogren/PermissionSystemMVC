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

        [Authorize(Roles = "Employee")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Create(ViewRequest request)
        {

            var nowYear = DateTime.Now.Year;
            var nowMonth = DateTime.Now.Month;
            var nowDay = DateTime.Now.Day;

            var requestsThisMonth = await  _context.Request.Where(i => i.CreatedById == User.GetUserId())
                .Where(d => d.CreateDate.Year == nowYear && d.CreateDate.Month == nowMonth &&
                d.PrmisssionType == Models.Request.PrmisssionTypeEnum.Personal &&
                d.Status == Models.Request.PrmisssionStatusEnum.New &&
                d.Status == Models.Request.PrmisssionStatusEnum.Approved).ToListAsync();

            var requestsThisDay =  requestsThisMonth
                .Where(d => d.CreateDate.Date == request.DatePrmission.Date).ToList();

            if (requestsThisDay.Any())
            {
                ModelState.AddModelError("", "You have Personal Request in this Day");
            }

            if (requestsThisMonth.Count >= 4)
            {
                ModelState.AddModelError("", "You have Personal Requests 4 Time in this month");
            }

            var TotalTime = request.ToTime - request.FromTime;

            if (request.FromTime >= request.ToTime)
            {
                ModelState.AddModelError("", "From Time must be less than To Time");
            }

            if (request.PrmisssionType == Models.Request.PrmisssionTypeEnum.Personal && TotalTime > 180)
            {
                ModelState.AddModelError("", "Personal leave Time must be less than or equls 3 Hours");
            }

            if (ModelState.IsValid)
            {
                var timeFrom = request.DatePrmission.AddMinutes(request.FromTime);
                var timeTo = request.DatePrmission.AddMinutes(request.ToTime);
                var requestNew = new Request
                {
                    PrmisssionType = request.PrmisssionType,
                    DatePrmission = request.DatePrmission,
                    FromTime = timeFrom,
                    ToTime = timeTo,
                    CreatedById = User.GetUserId(),
                    CreateDate = request.CreateDate,
                    Status = request.Status
                };
                _context.Request.Add(requestNew);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(request);
        }

        [Authorize(Roles = "Manager")]
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
        [Authorize(Roles = "Manager")]

        public async Task<IActionResult> ApproveConfirmed(int id)
        {
            var request = await _context.Request.FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            if (Equals(request.Status, Models.Request.PrmisssionStatusEnum.New) == false)
            {
                return BadRequest();
            }

            request.Status = Models.Request.PrmisssionStatusEnum.Approved;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Manager")]

        public async Task<IActionResult> Reject(int? id)
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

        [HttpPost, ActionName("Reject")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]

        public async Task<IActionResult> RejectConfirmed(int id)
        {
            var request = await _context.Request.FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
            {
                return NotFound();
            }


            if (Equals(request.Status, Models.Request.PrmisssionStatusEnum.New) == false)
            {
                return BadRequest();
            }

            request.Status = Models.Request.PrmisssionStatusEnum.Rejected;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Cancel(int? id)
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

        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]

        public async Task<IActionResult> CancelConfirmed(int id)
        {
            var request = await _context.Request.FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
            {
                return NotFound();
            }


            if (Equals(request.Status, Models.Request.PrmisssionStatusEnum.New) == false)
            {
                return BadRequest();
            }

            request.Status = Models.Request.PrmisssionStatusEnum.Canceled;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
