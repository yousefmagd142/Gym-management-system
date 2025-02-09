namespace Gym_System.Models
{
    public class CheckOut
    {
        public int Id { get; set; }
        public DateTime CheckOutTime { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; } // Navigation Property
    }
}
