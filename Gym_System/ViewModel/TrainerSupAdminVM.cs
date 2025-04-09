using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gym_System.ViewModel
{
    public class TrainerSupAdminVM
    {
        public string Id { get; set; }
        public DateTime StartDate { get; set; }= DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now;
        public List<SelectListItem> TrainerSupAdminList { get; set; } = new List<SelectListItem>();
        public List<AttendanceReportItem> AttendanceReport { get; set; } = new List<AttendanceReportItem>();
    }

    public class AttendanceReportItem
    {
        public DateTime Date { get; set; }
        public double TotalHours { get; set; }
        public double TotalMinutes { get; set; }

    }
}
