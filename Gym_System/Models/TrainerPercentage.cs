using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gym_System.Models
{
    public class TrainerPercentage
    {
        public int Id { get; set; }
        [Range(0, 100)]
        public decimal percentage { get; set; }

        public string TrainerId { get; set; }

        [ForeignKey(nameof(TrainerId))]
        public ApplicationUser Trainer { get; set; }=new ApplicationUser();
        
    }
}
