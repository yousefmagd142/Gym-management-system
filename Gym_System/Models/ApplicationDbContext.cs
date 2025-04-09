using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Gym_System.Models
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options){}
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Membrtship> Membrtships { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Expenses> Expenses { get; set; }
        public DbSet<TrainerPercentage> TrainerPercentages { get; set; }
        public DbSet<ClintDiscount> ClintDiscounts { get; set; }
        public DbSet<CheckOut> CheckOuts { get; set; }
        public DbSet<Freeze> Freezes { get; set; }
    }
}
