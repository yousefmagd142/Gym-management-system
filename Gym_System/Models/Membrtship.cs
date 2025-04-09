namespace Gym_System.Models
{
    public class Membrtship
    {
        public int Id { get; set; }
        public string Name { get; set; }=string.Empty;
        public Int16 DurationInDays { get; set; }
        public Int16 MaxVisits { get; set; }
        public decimal Price { get; set; }
        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }
}
