using System.ComponentModel.DataAnnotations;

namespace Gym_System.ViewModel
{
    public class LogInVM
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }

    }
}
