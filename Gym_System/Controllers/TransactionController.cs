using Gym_System.Models;
using Gym_System.Repository;
using Gym_System.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Gym_System.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        
        private readonly ApplicationDbContext _db;
        private readonly IRegistraionRepo _userServices;
        public TransactionController(ApplicationDbContext Db, IRegistraionRepo userServices)
        {
            _db = Db;
            _userServices = userServices;
        }
        [HttpGet]
        public async Task<IActionResult> AddTransaction()
        {
            ViewBag.Users = await _userServices.GetUserNameListAsync();
            TransactionViewModel viewModel = new TransactionViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTransaction(TransactionViewModel model)
        {
            ViewBag.Users = await _userServices.GetUserNameListAsync();
            if (ModelState.IsValid)
            {
                var transaction = new Transaction
                {
                    Paid = model.Paid,
                    Description = model.Description,
                    DateTime = model.DateTime,
                    UserId = model.UserId,
                };

                await _db.Transactions.AddAsync(transaction); // Add the transaction to the database

                var user = await _db.ApplicationUsers.FindAsync(transaction.UserId);
                if (user != null)
                {
                    user.Balance -= transaction.Paid; // Assuming "Paid" affects the balance positively
                    _db.ApplicationUsers.Update(user);
                }
                await _db.SaveChangesAsync(); 
                if (user.Balance <= 0) 
                {
                    user.MembershipState = "Active";
                    _db.ApplicationUsers.Update(user);
                    await _db.SaveChangesAsync(); 
                }
                ModelState.Clear();

                return RedirectToAction("AddTransaction"); // Redirect to a list or summary page
            }
            // If model validation fails, return the same view with the model
            return View("AddTransaction",model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var transaction = await _db.Transactions.FindAsync(id);
            if (transaction != null)
            {
                // Update user balance
                var user = await _db.Users.FindAsync(transaction.UserId);
                if (user != null)
                {
                    user.Balance += transaction.Paid; // Subtract the amount when deleting
                    _db.Users.Update(user);
                }

                _db.Transactions.Remove(transaction);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("SearchTransactionssearch");
        }

        [HttpGet]
        public async Task<IActionResult> SearchTransactions()
        {
            ViewBag.Users = await _userServices.GetUserNameListAsync();
            TransactionViewModel viewModel = new TransactionViewModel();
            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> SearchTransactionssearch(TransactionViewModel model)
        {
            var transactions = _db.Transactions.Include(t=>t.User).AsQueryable();
            // Filter by Start Date
            if (model.StartDate.HasValue)
            {
                transactions = transactions.Where(t => t.DateTime.Date >= model.StartDate.Value.Date);
            }

            // Filter by End Date
            if (model.EndDate.HasValue)
            {
                transactions = transactions.Where(t => t.DateTime.Date <= model.EndDate.Value.Date);
            }

            // Filter by UserId if selected
            if (!string.IsNullOrEmpty(model.UserId))
            {
                transactions = transactions.Where(t => t.UserId == model.UserId);
            }

            var filteredTransactions = await transactions
                .Include(t => t.User) // Ensure User is included'
                .OrderByDescending(t => t.DateTime)
                .ToListAsync();
            decimal totalPaid = filteredTransactions.Sum(t => t.Paid); // Summing the Amount field

            // Populate ViewBag.Users for the dropdown
            ViewBag.Users = await _userServices.GetUserNameListAsync();

            // Prepare the ViewModel with filtered transactions and search criteria
            var viewModel = new TransactionViewModel
            {
                FilteredTransactions = filteredTransactions,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                UserId = model.UserId, // Preserve the selected UserId
                totalpaid=totalPaid,
            };

            return View("SearchTransactions",viewModel);
        }

    }
}
