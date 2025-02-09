using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Gym_System.ViewModel
{
    public class RegistrationUserModel
    {
        public string Id { get; set; }
        public string Name { get; set; }=string.Empty;
        [EmailAddress]
        public string? Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\d{1,15}$", ErrorMessage = "Phone number must be numbers only and up to 15 digits.")]
        [StringLength(15, ErrorMessage = "Phone number must be at most 15 digits.")]
        public string PhoneNum { get; set; }

        public string UserName { get; set; }= string.Empty;

        [DataType(DataType.Password)]
        [Required]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must be at least 8 characters long and include an uppercase letter, a number, and a special character.")]
        public string Password { get; set; }=string.Empty ;

        [DataType(DataType.Password)]
        [Required]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
        [Required(ErrorMessage = "Please select a role.")]
        public string Role { get; set; } = string.Empty;
        public string? TrainerId { get; set; }
        public DateTime MembershipStartDate { get; set; } =DateTime.Now;
        public int? MembrtshipsId { get; set; } = null;
        public int AllowDays { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Discount must be a positive number.")]
        public int Discount { get; set; }

        public List<SelectListItem> MembershipsList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> TrainerList { get; set; } = new List<SelectListItem>();


    }
}
