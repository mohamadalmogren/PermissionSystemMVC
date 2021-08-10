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
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PermissionSystemMVC.Controllers
{
    [Authorize(Roles = "Manager")]

    public class ManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public ManagerController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            var requests = await _context.Request
                .Include(r => r.CreatedBy)
                .Where(r => r.CreatedBy.DepartmentId == user.DepartmentId)
                .ToListAsync();
            return View(requests);

        }

        [HttpGet]
        public async Task<ActionResult> GetAllRequests()
        {
            var retrunList = new List<ListRequestViewModel>();

            var user = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

            var requests = await _context.Request
                .Include(r => r.CreatedBy)
                .Where(r => r.CreatedBy.DepartmentId == user.DepartmentId)
                .ToListAsync();

            foreach (var item in requests)
            {
                retrunList.Add(new ListRequestViewModel
                {
                    Id = item.Id.ToString(),
                    CreatedBy = item.CreatedBy.Name,
                    PrmisssionType = item.PrmisssionType.ToString(),
                    DatePrmission = item.DatePrmission.ToString("dd/MM/yyyy"),
                    FromTime = item.FromTime.ToString("HH:mm"),
                    ToTime = item.ToTime.ToString("HH:mm"),
                    CreateDate = item.CreateDate.ToString("dd/MM/yyyy"),
                    Status = item.Status.ToString()
                });
            }
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() },
            };

            return Json(new { data = retrunList }, options);
        }

        [HttpPost]
        public async Task<JsonResult> ApproveRequest(int id)
        {
            var request = await _context.Request.FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
            {
                return Json(new { resulte = false, msg = "Request Not Found!" });
            }

            if (Equals(request.Status, Models.Request.PrmisssionStatusEnum.New) == false)
            {
                return Json(new { resulte = false, msg = "You cannot Approve this !!" });
            }

            request.Status = Models.Request.PrmisssionStatusEnum.Approved;

            await _context.SaveChangesAsync();

            return Json(new { resulte = true, msg = "Request Aproved" });
        }


        [HttpPost]
        public async Task<JsonResult> RejectRequest(int id)
        {
            var request = await _context.Request.FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
            {
                return Json(new { resulte = false, msg = "Request Not Found!" });
            }


            if (Equals(request.Status, Models.Request.PrmisssionStatusEnum.New) == false)
            {
                return Json(new { resulte = false, msg = "You cannot Reject this !!" });
            }

            request.Status = Models.Request.PrmisssionStatusEnum.Rejected;
            await _context.SaveChangesAsync();

            return Json(new { resulte = true, msg = "Request Rejected!" });
        }
    }
}
