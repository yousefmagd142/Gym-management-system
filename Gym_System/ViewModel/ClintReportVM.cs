using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gym_System.ViewModel
{
    public class ClientReportVM
    {
        public string ClientName { get; set; }=string.Empty;
        public string MembershipName { get; set; } = string.Empty;
        public DateTime MembershipStartDate { get; set; }
        public DateTime MembershipEndDate { get; set; }
        public int? FreezDays { get; set; }
        public int RemainDays { get; set; }
        public decimal MembershipPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal Paid { get; set; }
        public string ClientPhone { get; set; } // Client's phone number for WhatsApp

        public List<SelectListItem> ClientsList { get; set; }=new List<SelectListItem>();
    }
}
