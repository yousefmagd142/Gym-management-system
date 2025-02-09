using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Gym_System.ViewModel
{
    public class TrainerVM
    {
        public string TrainerId { get; set; }
        [Range(0, 100)]
        public int TrainerPercentage { get; set; }

        public List<SelectListItem> TrainerList { get; set; } = new List<SelectListItem>();
    }
}
