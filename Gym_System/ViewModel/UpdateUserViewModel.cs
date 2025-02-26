using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Gym_System.ViewModel
{
    public class UpdateUserViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; } = string.Empty;
        [EmailAddress]
        public string? Email { get; set; } = string.Empty;
        public string PhoneNum { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int AllowDays { get; set; }
        public decimal Balance { get; set; }
        public DateTime JoinDate { get; set; } = DateTime.Now;
        public DateTime MembershipStartDate { get; set; } = DateTime.Now;
        [Range(0, int.MaxValue, ErrorMessage = "Discount must be a positive number.")]
        public decimal Discount { get; set; }
        public string TarinerId { get; set; }
        public int MembershipId { get; set; }
        public List<SelectListItem> Names { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> TrainerNames { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Memberships { get; set; } = new List<SelectListItem>();

    }
}
