using Gym_System.Models;
using Gym_System.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Gym_System.Controllers
{
    [Authorize]
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> AddAttendance()
        {
            ViewBag.Users = await _context.Users
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id
                })
                .ToListAsync();

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddAttendancepost([FromForm] string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                ModelState.AddModelError("", "Please select a user.");
                return RedirectToAction(nameof(AddAttendance));
            }

            var checkin = new Attendance
            {
                CheckInTime = DateTime.Now,
                UserId = Id
            };

            _context.Attendances.Add(checkin);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(AddAttendance));
        }

        // GET: Attendance
        [HttpGet]
        public async Task<ActionResult> AttendanceGetReport()
        {
            // Fetch all users for the dropdown
            ViewBag.Users = await _context.Users
                            .Select(u => new SelectListItem
                            {
                                Text = u.Name,
                                Value = u.Id
                            }).ToListAsync();
            var model = new AttendanceVM(); 
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Filter(AttendanceVM model)
        {
            var attencances = _context.Attendances.AsQueryable();

            // Filter by Start Date
            if (model.StartDate.HasValue)
            {
                attencances = attencances.Where(t => t.CheckInTime.Date >= model.StartDate.Value.Date);
            }

            // Filter by End Date
            if (model.EndDate.HasValue)
            {
                attencances = attencances.Where(t => t.CheckInTime.Date <= model.EndDate.Value.Date);
            }

            // Filter by UserId if selected
            if (!string.IsNullOrEmpty(model.UserId))
            {
                attencances = attencances.Where(t => t.UserId == model.UserId);
            }


            var filtter = await attencances
                .Include(t => t.User) // Ensure User is included
                .OrderByDescending(t => t.CheckInTime)
                .ToListAsync();

            // Prepare the ViewModel with filtered transactions and search criteria
            var viewModel = new AttendanceVM
            {
                FilteredAttendancess = filtter,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                UserId = model.UserId // Preserve the selected UserId
            };
            ViewBag.Users = await _context.Users
                           .Select(u => new SelectListItem
                           {
                               Text = u.Name,
                               Value = u.Id
                           }).ToListAsync();
            return View("AttendanceGetReport", viewModel);
        }
    }
}

