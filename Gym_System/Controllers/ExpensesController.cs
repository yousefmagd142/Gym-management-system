using Gym_System.Models;
using Gym_System.Repository;
using Gym_System.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gym_System.Controllers
{
    public class ExpensesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IRegistraionRepo _userServices;
        public ExpensesController(ApplicationDbContext Db, IRegistraionRepo userServices)
        {
            _db = Db;
            _userServices = userServices;
        }

        [HttpGet]
        public async Task<IActionResult> AddExpenses()
        {
            ExpensesViewMode viewModel = new ExpensesViewMode();
            viewModel.UsersList = await _userServices.GetTrainersAndSubAdminAsync();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddExpenses(ExpensesViewMode model)
        {
            model.UsersList = await _userServices.GetTrainersAndSubAdminAsync();
            if (ModelState.IsValid)
            {
                var expensess = new Expenses
                {
                    MoneyTaken = model.MoneyTaken,
                    Description = model.Description,
                    DateTime = model.DateTime,
                    UserId = model.UserId,
                };

                await _db.Expenses.AddAsync(expensess); // Add the transaction to the database

                var user = await _db.ApplicationUsers.FindAsync(expensess.UserId);
                if (user != null)
                {
                    user.Balance -= expensess.MoneyTaken; // Assuming "Paid" affects the balance positively
                    _db.ApplicationUsers.Update(user);
                }
                await _db.SaveChangesAsync();
                ModelState.Clear();
                return RedirectToAction("AddExpenses"); // Redirect to a list or summary page
            }
            // If model validation fails, return the same view with the model
            return View("AddExpenses", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteExpenses(int id)
        {
            var expenses = await _db.Expenses.FindAsync(id);
            if (expenses != null)
            {
                // Update user balance
                var user = await _db.ApplicationUsers.FindAsync(expenses.UserId);
                if (user != null)
                {
                    user.Balance += expenses.MoneyTaken; // Subtract the amount when deleting
                    _db.ApplicationUsers.Update(user);
                }

                _db.Expenses.Remove(expenses);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("SearchExpensessearch");
        }
        [HttpGet]
        public async Task<IActionResult> SearchExpenses()
        {
            ExpensesViewMode viewModel = new ExpensesViewMode();
           viewModel.UsersList = await _userServices.GetTrainersAndSubAdminAsync();
            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> SearchExpensessearch(ExpensesViewMode model)
        {
            var expenses = _db.Expenses.Include(t => t.User).AsQueryable();
            // Filter by Start Date
            if (model.StartDate.HasValue)
            {
                expenses = expenses.Where(t => t.DateTime.Date >= model.StartDate.Value.Date);
            }

            // Filter by End Date
            if (model.EndDate.HasValue)
            {
                expenses = expenses.Where(t => t.DateTime.Date <= model.EndDate.Value.Date);
            }

            // Filter by UserId if selected
            if (!string.IsNullOrEmpty(model.UserId))
            {
                expenses = expenses.Where(t => t.UserId == model.UserId);
            }

            var filteredTransactions = await expenses
                .Include(t => t.User) // Ensure User is included'
                .OrderByDescending(t => t.DateTime)
                .ToListAsync();
            decimal totaltaken = filteredTransactions.Sum(t => t.MoneyTaken); // Summing the Amount field

            // Populate ViewBag.Users for the dropdown
            model.UsersList = await _userServices.GetTrainersAndSubAdminAsync();

            // Prepare the ViewModel with filtered transactions and search criteria
            var viewModel = new ExpensesViewMode
            {
                FilteredTransactions = filteredTransactions,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                UserId = model.UserId, // Preserve the selected UserId
                totaltaken = totaltaken,
                UsersList = await _userServices.GetTrainersAndSubAdminAsync(),
            };

            return View("SearchExpenses", viewModel);
        }

    }
}

