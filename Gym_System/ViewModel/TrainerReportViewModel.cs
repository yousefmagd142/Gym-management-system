using Gym_System.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gym_System.ViewModel
{
    public class TrainerReportViewModel
    {
        public string TrainerName { get; set; }
        public decimal TotalMembershipRevenue { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal Trainerearnings { get; set; }
        public decimal TotalPaid { get; set; }
        public List<ClientReportVM> Clients { get; set; } = new List<ClientReportVM>();

        public List<SelectListItem> TrainerList { get; set; } = new List<SelectListItem>();
    }

}
