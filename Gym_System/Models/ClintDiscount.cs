using System.ComponentModel.DataAnnotations.Schema;

namespace Gym_System.Models
{
    public class ClintDiscount
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Clint))]
        public string ClintId { get; set; }
        public decimal Discount { get; set; }
        public ApplicationUser Clint { get; set; } = new ApplicationUser();
    }
}
