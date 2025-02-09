using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gym_System.ViewModel
{
    public class MembershipVM
    {
        public string Name { get; set; } = string.Empty;
        public Int16 DurationInDays { get; set; }
        public Int16 MaxVisits { get; set; }
        public decimal Price { get; set; }
        public List<SelectListItem> MembershipsList { get; set; } = new List<SelectListItem>();
    }
}
