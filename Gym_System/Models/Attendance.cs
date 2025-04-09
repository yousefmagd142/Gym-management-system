namespace Gym_System.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        public DateTime CheckInTime { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; } // Navigation Property
    }
}
