using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Gym_System.ViewModel
{
    public class RenewMembershipVM
    {
        public string SelectedUserId { get; set; }
        public int SelectedMembershipId { get; set; }
        public List<SelectListItem> User { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Membership { get; set; } = new List<SelectListItem>();
        public int AllowDays { get; set; }
        public DateTime MembershipStartDate { get; set; } = DateTime.Now;
        [Range(0, int.MaxValue, ErrorMessage = "Discount must be a positive number.")]
        public int Discount { get; set; }
        public decimal PaidTransaction { get; set; }
        public string DescriptionTransaction { get; set; } = "First Payment";
    }
}
