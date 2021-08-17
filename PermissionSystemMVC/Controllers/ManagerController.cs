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
        public async Task<IActionResult> History()
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
                .Where(r => r.Status == Models.Request.PrmisssionStatusEnum.Canceled == false)
                .ToListAsync();

            foreach (var item in requests)
            {
                retrunList.Add(new ListRequestViewModel
                {
                    Id = item.Id.ToString(),
                    CreatedBy = item.CreatedBy.Name,
                    CreatedById = item.CreatedBy.Id,
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

        [HttpGet]
        public async Task<ActionResult> GetAllRequestsIndex()
        {
            var retrunList = new List<ListRequestViewModel>();            

            var user = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

            var requests = await _context.Request
                .Include(r => r.CreatedBy)
                .Where(r => r.CreatedBy.DepartmentId == user.DepartmentId)
                .Where(r => r.Status == Models.Request.PrmisssionStatusEnum.New)
                .Where(r => r.DatePrmission.Month == DateTime.Now.Month)
                .ToListAsync();

            foreach (var item in requests)
            {
                retrunList.Add(new ListRequestViewModel
                {
                    Id = item.Id.ToString(),
                    CreatedBy = item.CreatedBy.Name,
                    CreatedById = item.CreatedBy.Id,
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


        [HttpGet]
        public async Task<ActionResult> GetUserList()
        {
            var ListRequestsPerUser = new List<IndexManagerViewModel>();

            var user = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

            var requests = await _context.Request
                .Include(r => r.CreatedBy)
                .Where(r => r.CreatedBy.DepartmentId == user.DepartmentId)
                .Where(r => r.Status == Models.Request.PrmisssionStatusEnum.Canceled == false)
                .Where(r => r.DatePrmission.Month == DateTime.Now.Month)
                .ToListAsync();

            var UsersInDepartment = await _context.Users
               .Where(r => r.DepartmentId == user.DepartmentId)
               .Where(r => r.Id == User.GetUserId() == false)
               .Select(r => r.Id)
               .ToListAsync();

            var TotalTime = 0;
            var countApproved = 0;
            var countRejected = 0;
            var countNew = 0;

            for (int i = 0; i < UsersInDepartment.Count; i++)
            {
                var UserRequests = requests
                    .Where(r => r.CreatedById == UsersInDepartment[i])
                    .ToList();

                var Approved = UserRequests
                    .Where(r => r.Status == Models.Request.PrmisssionStatusEnum.Approved)
                    .ToList();
             
                var Reject = UserRequests
                    .Where(r => r.Status == Models.Request.PrmisssionStatusEnum.Rejected)
                    .ToList();

                var New = UserRequests
                    .Where(r => r.Status == Models.Request.PrmisssionStatusEnum.New)
                    .ToList();
                
                countApproved = Approved.Count;
                countRejected = Reject.Count;
                countNew = New.Count;
                
                var UserRequestsCount = UserRequests.Count;

                foreach (var item in Approved)
                {
                    var TotalFromTime = (item.FromTime.Hour * 60) + item.FromTime.Minute;
                    var TotalToTime = (item.ToTime.Hour * 60) + item.ToTime.Minute;
                    var Total = TotalToTime - TotalFromTime;
                    TotalTime = TotalTime + Total;
                }
                var Time = TimeSpan.FromMinutes(TotalTime);


                ListRequestsPerUser.Add(new IndexManagerViewModel
                {
                    Requests = UserRequestsCount.ToString(),
                    Hours = (int)Time.TotalHours + ":" + Time.Minutes,
                    Name = UserRequests[0].CreatedBy.Name.ToString(),
                    Approved = countApproved.ToString(),
                    Rejected = countRejected.ToString(),
                    New = countNew.ToString()
                });

                TotalTime = 0;
                countApproved = 0;
                countRejected = 0;
                countNew = 0;
            }

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() },
            };

            return Json(new { data = ListRequestsPerUser }, options);
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
