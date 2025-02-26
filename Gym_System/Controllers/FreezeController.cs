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
    public class FreezeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public FreezeController(IRegistraionRepo registraionRepo,ApplicationDbContext Db)
        {
            _db = Db;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new FreezeVM
            {
                ClientsList = await (from user in _db.ApplicationUsers
                                     join userRole in _db.UserRoles on user.Id equals userRole.UserId
                                     join role in _db.Roles on userRole.RoleId equals role.Id
                                     where role.Name == "Client"
                                     select new SelectListItem
                                     {
                                         Value = user.Id,
                                         Text = user.Name
                                     }).ToListAsync()
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Index(FreezeVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _db.ApplicationUsers.FirstOrDefaultAsync(i => i.Id == model.UserId);
            if (user == null)
            {
                return NotFound(); // Handle case where user is not found
            }
            var froozen=await _db.Freezes.FirstOrDefaultAsync(i=>i.UserId == model.UserId);
            if (froozen == null)
            {
                string sql = @"
                INSERT INTO Freezes (UserId, MemberShepStartDate, FreezeDays)
                VALUES (@UserId, @MemberShepStartDate, @FreezeDays)";

                await _db.Database.ExecuteSqlRawAsync(sql,
                    new SqlParameter("@UserId", model.UserId),
                    new SqlParameter("@MemberShepStartDate", user.MembershipStartDate),
                    new SqlParameter("@FreezeDays", model.FreezeDays)
                );
            }
            else if (froozen != null)
            {
                string sql = @"
                    UPDATE Freezes 
                    SET MemberShepStartDate = @MemberShepStartDate, 
                        FreezeDays = @FreezeDays
                    WHERE UserId = @UserId";

                await _db.Database.ExecuteSqlRawAsync(sql,
                    new SqlParameter("@UserId", model.UserId),
                    new SqlParameter("@MemberShepStartDate", user.MembershipStartDate),
                    new SqlParameter("@FreezeDays", model.FreezeDays)
                );
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public JsonResult GetClientNames(string term)
        {
            var clientNames = _db.ApplicationUsers
                .Where(c => c.Name.Contains(term))  // Filter by input text
                .Select(c => c.Name)
                .ToList();

            return Json(clientNames);
        }

    }
}
