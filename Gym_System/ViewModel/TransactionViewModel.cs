using Gym_System.Models;

namespace Gym_System.ViewModel
{
    public class TransactionViewModel
    {
        public decimal Paid { get; set; }
        public string? Description { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }=DateTime.Now;
        public string UserId { get; set; }=string.Empty;

        public decimal totalpaid { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<Transaction> FilteredTransactions { get; set; } = new List<Transaction>();
        public ApplicationUser User { get; set; }=new ApplicationUser();
    }
}
