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
    [Authorize(Roles = "Employee")]

    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public EmployeeController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // Get Reuests for Employee
            var requests = await _context.Request.Where(r => r.CreatedById == User.GetUserId()).ToListAsync();

            return View(requests);
        }

        [HttpGet]
        public async Task<ActionResult> GetAllRequests()
        {
            var retrunList = new List<ListRequestViewModel>();
            
            var requests = await _context.Request
                .Include(r => r.CreatedBy)
                .Where(r => r.CreatedById == User.GetUserId())
                .ToListAsync();

            foreach (var item in requests)
            {
                retrunList.Add(new ListRequestViewModel
                {
                    Id = item.Id.ToString(),
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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ViewRequest request)
        {

            var nowYear = request.DatePrmission.Year;
            var nowMonth = request.DatePrmission.Month;
            var nowDay = request.DatePrmission.Day;

            var requestsThisMonth = await _context.Request.Where(i => i.CreatedById == User.GetUserId())
                .Where(d => d.DatePrmission.Year == nowYear && d.DatePrmission.Month == nowMonth &&
                d.Status == Models.Request.PrmisssionStatusEnum.New ||
                d.Status == Models.Request.PrmisssionStatusEnum.Approved).ToListAsync();

            var personalRequesMonth = requestsThisMonth.Where(
                p => p.PrmisssionType == Models.Request.PrmisssionTypeEnum.Personal).ToList();

            var requestsThisDay = requestsThisMonth
                .Where(d => d.DatePrmission.Date == request.DatePrmission.Date).ToList();

            var requestThisDayPersonal = requestsThisDay.Where(p => p.PrmisssionType == Models.Request.PrmisssionTypeEnum.Personal).ToList();


            if (personalRequesMonth.Count >= 4)
            {
                ModelState.AddModelError("", "You have Personal Requests 4 Time in this month");
            }

            if (requestsThisDay.Any())
            {
                for (int i = 0; i < requestsThisDay.Count; i++)
                {

                    var FromTimeDb = requestsThisDay[i].FromTime;
                    var ToTimeDb = requestsThisDay[i].ToTime;

                    var MinutesFromTimeDb = (FromTimeDb.Hour * 60) + FromTimeDb.Minute;
                    var MinutesToTimeDb = (ToTimeDb.Hour * 60) + ToTimeDb.Minute;

                    if (request.FromTime <= MinutesToTimeDb && request.FromTime >= MinutesFromTimeDb ||
                        request.ToTime >= MinutesFromTimeDb && request.ToTime <= MinutesToTimeDb)
                    {
                        ModelState.AddModelError("", "You have Request in this time");
                    }

                    if (requestThisDayPersonal[i].PrmisssionType == Models.Request.PrmisssionTypeEnum.Personal &&
                        request.PrmisssionType == Models.Request.PrmisssionTypeEnum.Personal)
                    {
                        ModelState.AddModelError("", "You have Personal Request in this Day");
                    }

                }
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

        [HttpPost]
        public async Task<JsonResult> CancelRequest(int id)
        {
            var request = await _context.Request.FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
            {
                return Json(new { resulte = false, msg = "Request Not Found!" });
            }


            if (Equals(request.Status, Models.Request.PrmisssionStatusEnum.New) == false)
            {
                return Json(new { resulte = false, msg = "You cannot Cancel this !!" });

            }

            request.Status = Models.Request.PrmisssionStatusEnum.Canceled;
            await _context.SaveChangesAsync();

            return Json(new { resulte = true, msg = "Request Canceled!" });
        }

    }
}
