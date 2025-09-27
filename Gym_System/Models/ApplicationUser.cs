using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Gym_System.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class ApplicationUser:IdentityUser
    {
        
        public string Name { get; set; }=string.Empty;
        public DateTime JoinDate { get; set; }

        public int? MembrtshipsId { get; set; } = null;
        public Membrtship? Membrtships { get; set;}
        public Freeze? Freezes { get; set; }
        public DateTime MembershipStartDate { get; set; }=DateTime.Now;
        public string? MembershipState { get; set; }="NotActive";
        public decimal Balance { get; set; } = 0;
        public int AllowDays { get; set; } = 0;


        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        public string? TrainerId { get; set; } // Foreign key to the trainer
        public ApplicationUser? Trainer { get; set; } // Navigation property for the trainer
        public ICollection<ApplicationUser> Clients { get; set; } = new List<ApplicationUser>(); // Navigation property for clients
    }
}
