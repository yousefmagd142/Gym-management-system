using Gym_System.Models;
using Gym_System.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Gym_System.Controllers
{
    [Authorize]
    public class ClintController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ClintController(ApplicationDbContext Db)
        {
            _db = Db;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string clientId )
        {
            // Prepare the ViewModel
            var viewModel = new ClientReportVM();

            // Populate the dropdown list with all clients
            viewModel.ClientsList = await (from user in _db.ApplicationUsers
                                        join userRole in _db.UserRoles on user.Id equals userRole.UserId
                                        join role in _db.Roles on userRole.RoleId equals role.Id
                                        where role.Name == "Client"
                                        select new SelectListItem
                                        {
                                            Value = user.Id,
                                            Text = user.Name
                                        }).ToListAsync();

            // If a client is selected, fetch their details
            if (!string.IsNullOrEmpty(clientId))
            {
                var client = await _db.ApplicationUsers
                    .Include(c => c.Membrtships)
                    .FirstOrDefaultAsync(c => c.Id == clientId);

                if (client != null)
                {
                    int? freezdays = await _db.Freezes.Where(i => i.UserId == clientId)
                        .Select(c => c.FreezeDays)
                        .FirstOrDefaultAsync();
                    decimal? discount = await _db.ClintDiscounts
                        .Where(cd => cd.ClintId == clientId)
                        .Select(cd => cd.Discount)
                        .FirstOrDefaultAsync();

                    // Fetch membership details
                    viewModel.ClientName = client.Name;
                    viewModel.MembershipName = client.Membrtships?.Name ?? "No Membership";
                    viewModel.MembershipPrice = client.Membrtships?.Price ?? 0;
                    viewModel.MembershipStartDate = client.MembershipStartDate ;
                    viewModel.ClientPhone = client.PhoneNumber;
                    viewModel.MembershipEndDate=client.MembershipStartDate.AddDays(client.Membrtships?.DurationInDays??0);
                    viewModel.FreezDays = freezdays ??0  ;
                    viewModel.Discount= discount??0;
                    viewModel.RemainDays = (viewModel.MembershipEndDate - DateTime.Now).Days;                    // Fetch discount


                    // Calculate total paid from transactions
                    viewModel.Paid = await _db.Transactions
                        .Where(t => t.UserId == clientId && t.DateTime >= client.MembershipStartDate)
                        .SumAsync(t => t.Paid);
                }
            }

            return View(viewModel);
        }

    }
}
