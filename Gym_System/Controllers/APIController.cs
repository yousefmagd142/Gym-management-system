using Gym_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Gym_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public APIController(ApplicationDbContext Db, UserManager<ApplicationUser> userManager)
        {
            _db = Db;
            _userManager = userManager;
        }

        [HttpGet("CheckId")]
        public async Task<IActionResult> CheckId(string id)
        
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new { exists = false, message = "ID cannot be null or empty." });
            }

            var checkuserfingerpring = await _db.Links.FirstOrDefaultAsync(u => u.FingerPring == id);
            if(checkuserfingerpring == null)
            {
                return RedirectToAction("RegisterUser", "AccountUser", new { id = id });
            }
            var user = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == checkuserfingerpring.UserId);
        
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Trainer") || roles.Contains("SubAdmin") || roles.Contains("Admin"))
                {
                    var checkin = new Attendance
                    {
                        CheckInTime = DateTime.Now,
                        UserId = user.Id
                    };
                    _db.Attendances.Add(checkin);
                    _db.SaveChanges();
                    return Ok(new { exists = true, message = "Employee." });
                }


                var userisfroozen = await _db.Freezes.FirstOrDefaultAsync(i => i.UserId == id && i.MemberShepStartDate == user.MembershipStartDate);
                var membership = await _db.Membrtships.FirstOrDefaultAsync(i => i.Id == user.MembrtshipsId);
                var membershipstartdate = user.MembershipStartDate;
                var membershipduration = membership.DurationInDays;
                var membershipmaxvisits = membership.MaxVisits;

                var attendanceDaysCount = await _db.Attendances
                    .Where(a => a.UserId == user.Id && a.CheckInTime.Date >= membershipstartdate.Date)
                    .Select(a => a.CheckInTime.Date)
                    .Distinct()
                    .CountAsync();

                if (userisfroozen != null && DateTime.Now < membershipstartdate.AddDays(membershipduration + userisfroozen.FreezeDays) && attendanceDaysCount < membership.MaxVisits)
                {
                    return Ok(new { exists = true, message = $"Client has freeze for {userisfroozen.FreezeDays} days." });
                }

                if (roles.Contains("Client") && user.MembershipState == "Active")
                {

                    if (DateTime.Now > membershipstartdate.AddDays(membershipduration) || attendanceDaysCount > membership.MaxVisits)
                    {
                        user.MembershipState = "NotActive";
                        await _db.SaveChangesAsync();
                        return Ok(new { exists = false, message = "MemberShip has finished or you have reached max visits." });
                    }
                    // Log or use the attendanceDaysCount as needed
                    var checkin = new Attendance
                    {
                        CheckInTime = DateTime.Now,
                        UserId = user.Id
                    };
                    _db.Attendances.Add(checkin);
                    await _db.SaveChangesAsync();

                    return Ok(new { exists = true, message = "User exists and Active." });

                }
                else if (roles.Contains("Client") && user.MembershipState == "NotActive" && DateTime.Now <= membershipstartdate.AddDays(user.AllowDays))
                {
                    return Ok(new { exists = true, message = $"User exists and has {user.AllowDays} allow days." });
                }
                else if (roles.Contains("Client") && user.MembershipState == "NotActive")
                {
                    return Ok(new { exists = false, message = "User exists and not active." });
                }
            }
            return RedirectToAction("RegisterUser", "AccountUser", new { id = id });
        }

        [HttpGet("CheckOut")]
        public async Task<IActionResult> CheckOut(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new { exists = false, message = "ID cannot be null or empty." });
            }
            var user = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == id);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Trainer") || roles.Contains("SubAdmin"))
                {
                    var checkin = new CheckOut
                    {
                        CheckOutTime = DateTime.Now,
                        UserId = user.Id
                    };
                    _db.CheckOuts.Add(checkin);
                    _db.SaveChanges();
                    return Ok(new { exists = true, message = "Employee." });
                }
                return Ok(new { exists = true, message = "Client." });
            }
            return Ok(new { exists = false, message = "Not Authoriezed." });
        }
    }
}
