using Gym_System.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gym_System.ViewModel
{
    public class AttendanceVM
    {
        public DateTime CheckInTime { get; set; }
        public string? UserId { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<Attendance> FilteredAttendancess { get; set; } = new List<Attendance>();
    }
}
