using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gym_System.ViewModel
{
    public class FreezeVM
    {
        public string UserId { get; set; }
        public int FreezeDays { get; set; }
        public DateTime Membershipstartdate { get; set; }
        public List<SelectListItem> ClientsList { get; set; }=new List<SelectListItem>();
    }
}
