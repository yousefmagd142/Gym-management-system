namespace Gym_System.Models
{
    public class Freeze
    {
        public int Id { get; set; }
        public string UserId { get; set; }=string.Empty;
        public DateTime MemberShepStartDate { get; set; }
        public int FreezeDays { get; set; }

        public ApplicationUser User { get; set; } = new ApplicationUser();
    }
}
