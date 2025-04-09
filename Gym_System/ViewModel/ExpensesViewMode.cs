using Gym_System.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gym_System.ViewModel
{
    public class ExpensesViewMode
    {
        public decimal MoneyTaken { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime DateTime { get; set; } = DateTime.Now;
        public string UserId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = new ApplicationUser();
        public decimal totaltaken { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<Expenses> FilteredTransactions { get; set; } = new List<Expenses>();
        public List<SelectListItem> UsersList { get; set; } = new List<SelectListItem>();
    }
}
