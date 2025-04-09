namespace Gym_System.Models
{
    public class Expenses
    {
        public int Id { get; set; }
        public decimal MoneyTaken { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
