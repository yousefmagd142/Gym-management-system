using Gym_System.Models;
using Gym_System.Repository;
using Gym_System.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Gym_System.Controllers
{
    [Authorize]
    public class TrainerController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ITrainerRepo _servece;
        public TrainerController(ApplicationDbContext Db, ITrainerRepo Servece)
        {
            _db = Db;
            _servece = Servece;
        }
        public async Task<IActionResult> TrainerReport()
        {
            var viewModel = new TrainerReportViewModel
            {
                TrainerList = await (from user in _db.ApplicationUsers
                                     join userRole in _db.UserRoles on user.Id equals userRole.UserId
                                     join role in _db.Roles on userRole.RoleId equals role.Id
                                     where role.Name == "Trainer"
                                     select new SelectListItem
                                     {
                                         Value = user.Id.ToString(),
                                         Text = user.Name
                                     }).ToListAsync()
            };
            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> TrainerReportReport(string trainerId, DateTime? startDate, DateTime? endDate)
        {
            if (string.IsNullOrEmpty(trainerId))
            {
                return RedirectToAction("TrainerReport");
            }

            var trainer = await _servece.GetTrainerById(trainerId);

            if (trainer == null)
            {
                return NotFound("Trainer not found");
            }

            var trainerPercentage = await _db.TrainerPercentages
                .Where(tp => tp.TrainerId == trainerId)
                .Select(tp => tp.percentage)
                .FirstOrDefaultAsync();

            var clients = await _db.ApplicationUsers
                .Where(c => c.TrainerId == trainerId &&
                            (startDate == null || c.MembershipStartDate.Date >= startDate) &&
                            (endDate == null || c.MembershipStartDate.Date <= endDate))
                .Include(c => c.Membrtships)
                .ToListAsync();

            var viewModel = new TrainerReportViewModel
            {
                TrainerName = trainer.Name,
                TotalMembershipRevenue = 0,
                Trainerearnings = 0,
                TrainerList = await (from user in _db.ApplicationUsers
                                     join userRole in _db.UserRoles on user.Id equals userRole.UserId
                                     join role in _db.Roles on userRole.RoleId equals role.Id
                                     where role.Name == "Trainer"
                                     select new SelectListItem
                                     {
                                         Value = user.Id.ToString(),
                                         Text = user.Name
                                     }).ToListAsync()
            };

            foreach (var client in clients)
            {
                if (client.Membrtships != null)
                {
                    var discount = await _db.ClintDiscounts
                        .Where(cd => cd.ClintId == client.Id)
                        .Select(cd => cd.Discount)
                        .FirstOrDefaultAsync();
                    var paid = await _db.Transactions
                        .Where(t => t.UserId == client.Id && t.DateTime >= client.MembershipStartDate)
                        .SumAsync(t => t.Paid);

                    viewModel.Clients.Add(new ClientReportVM
                    {
                        ClientName = client.Name,
                        MembershipName = client.Membrtships.Name,
                        MembershipPrice = client.Membrtships.Price,
                        MembershipStartDate = client.MembershipStartDate,
                        Discount = discount,
                        Paid = paid
                    });
                    viewModel.TotalPaid += paid;
                    viewModel.TotalMembershipRevenue += client.Membrtships.Price;
                    viewModel.TotalDiscount += discount;
                }
            }

            viewModel.Trainerearnings = viewModel.TotalMembershipRevenue * trainerPercentage / 100 - viewModel.TotalDiscount;
            return View("TrainerReport", viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> AddTrainerPercentage()
        {
            var model = await _servece.GetTrainerList();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AddTrainerPercentage(TrainerVM model)
        {
            if (!ModelState.IsValid)
            {
                model = await _servece.GetTrainerList();
                return View("AddTrainerPercentage", model);
            }

            if (string.IsNullOrEmpty(model.TrainerId) || model.TrainerPercentage < 0)
            {
                ModelState.AddModelError("", "Invalid Trainer ID or Percentage.");
                model = await _servece.GetTrainerList();
                return View("AddTrainerPercentage", model);
            }

            var existingTrainer = await _servece.isTrainerPercentageExist(model.TrainerId);

            if (existingTrainer)
            {
                ModelState.AddModelError("", "A percentage for this trainer already exists.");
                model = await _servece.GetTrainerList();
                return View("AddTrainerPercentage", model);
            }

            try
            {
                await _servece.AddTrainerPercentage(model.TrainerId, model.TrainerPercentage);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                model = await _servece.GetTrainerList();
                return View("AddTrainerPercentage", model);
            }
            return RedirectToAction("AddTrainerPercentage");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateTrainerPercentage(TrainerVM model)
        {
            if (!ModelState.IsValid)
            {
                model = await _servece.GetTrainerList();
                return View("AddTrainerPercentage", model);
            }

            if (string.IsNullOrEmpty(model.TrainerId) || model.TrainerPercentage < 0)
            {
                ModelState.AddModelError("", "Invalid Trainer ID or Percentage.");
                model = await _servece.GetTrainerList();
                return View("AddTrainerPercentage", model);
            }

            var existingTrainer = await _servece.isTrainerPercentageExist(model.TrainerId);

            if (existingTrainer)
            {
                await _servece.UpdateTrainerPercentage(model.TrainerId, model.TrainerPercentage);
                return RedirectToAction("AddTrainerPercentage");
            }
            try
            {
                await _servece.AddTrainerPercentage(model.TrainerId, model.TrainerPercentage);

            }
            catch (Exception ex)
            {
                model = await _servece.GetTrainerList();
                return View("AddTrainerPercentage", model);
            }
            return RedirectToAction("AddTrainerPercentage");
        }

        [HttpGet]
        public async Task<IActionResult> TrainerSupAdminAttendanceReport()
        {
            TrainerSupAdminVM model=new TrainerSupAdminVM
            {
                TrainerSupAdminList = await (from user in _db.ApplicationUsers
                                             join userRole in _db.UserRoles on user.Id equals userRole.UserId
                                             join role in _db.Roles on userRole.RoleId equals role.Id
                                             where role.Name == "Trainer" || role.Name == "SupAdmin"
                                             select new SelectListItem
                                             {
                                                 Value = user.Id.ToString(),
                                                 Text = user.Name
                                             }).ToListAsync()
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> TrainerSupAdminAttendanceReport(TrainerSupAdminVM model)
        {
            if (model.StartDate > model.EndDate)
            {
                ModelState.AddModelError("", "Start date cannot be after the end date.");
                return View(model);
            }

            if (string.IsNullOrEmpty(model.Id))
            {
                ModelState.AddModelError("", "Trainer/SupAdmin must be selected.");
                return View(model);
            }

            // Query attendance and check-out records using EF Core
            var attendanceData = await _db.Attendances
                .Where(ci => ci.UserId == model.Id &&
                             ci.CheckInTime >= model.StartDate &&
                             ci.CheckInTime <= model.EndDate.AddDays(1)) // Include late-night check-outs
                .Select(ci => new
                {
                    ci.UserId,
                    CheckInTime = ci.CheckInTime
                })
                .ToListAsync();

            var checkoutData = await _db.CheckOuts
                .Where(co => co.UserId == model.Id &&
                             co.CheckOutTime >= model.StartDate &&
                             co.CheckOutTime <= model.EndDate.AddDays(1)) // Include late-night check-outs
                .Select(co => new
                {
                    co.UserId,
                    CheckOutTime = co.CheckOutTime
                })
                .ToListAsync();

            // Join Check-In and Check-Out data and calculate durations
            var report = attendanceData
                .Select(ci =>
                {
                    // Find the closest matching CheckOut for the current CheckIn
                    var co = checkoutData
                        .Where(co => co.UserId == ci.UserId && co.CheckOutTime >= ci.CheckInTime)
                        .OrderBy(co => co.CheckOutTime) // Get the earliest valid CheckOut
                        .FirstOrDefault();

                    // Calculate the duration
                    var duration = co != null
                        ? (co.CheckOutTime - ci.CheckInTime).TotalMinutes
                        : 0; // Handle missing CheckOut

                    return new
                    {
                        CheckInDate = ci.CheckInTime.Date,
                        TotalMinutes = duration
                    };
                })
                .GroupBy(x => x.CheckInDate)
                .Select(g => new AttendanceReportItem
                {
                    Date = g.Key,
                    TotalHours = Math.Floor(g.Sum(x => x.TotalMinutes) / 60), // Convert minutes to hours
                    TotalMinutes = g.Sum(x => x.TotalMinutes) % 60 // Remaining minutes
                })
                .OrderBy(x => x.Date)
                .ToList();

            // Populate the ViewModel
            var viewModel = new TrainerSupAdminVM
            {
                Id = model.Id,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                AttendanceReport = report,
                TrainerSupAdminList = await _db.ApplicationUsers
                    .Join(
                        _db.UserRoles,
                        user => user.Id,
                        userRole => userRole.UserId,
                        (user, userRole) => new { user, userRole }
                    )
                    .Join(
                        _db.Roles,
                        ur => ur.userRole.RoleId,
                        role => role.Id,
                        (ur, role) => new { ur.user, role }
                    )
                    .Where(x => x.role.Name == "Trainer" || x.role.Name == "SupAdmin")
                    .Select(x => new SelectListItem
                    {
                        Value = x.user.Id.ToString(),
                        Text = x.user.Name
                    })
                    .ToListAsync()
            };

            return View("TrainerSupAdminAttendanceReport", viewModel);
        }

        public async Task<IActionResult> TrainerDetails(string id)
        {
            var trainer = await _db.TrainerPercentages.FirstOrDefaultAsync(i=>i.TrainerId==id);

            if (trainer == null)
            {
                return NotFound();
            }
            return Json(trainer);
        }

    }
}
