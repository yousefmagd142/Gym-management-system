using Gym_System.Models;
using Gym_System.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Gym_System.Repository
{
    public class TrainerRepo:ITrainerRepo
    {
        private readonly ApplicationDbContext _db;
        public TrainerRepo(ApplicationDbContext Db)
        {
            _db = Db;
        }
        public async Task<TrainerVM> GetTrainerList()
        {
            var model = new TrainerVM();
            model.TrainerList = await (from user in _db.ApplicationUsers
                                       join userRole in _db.UserRoles on user.Id equals userRole.UserId
                                       join role in _db.Roles on userRole.RoleId equals role.Id
                                       where role.Name == "Trainer"
                                       select new SelectListItem
                                       {
                                           Value = user.Id.ToString(),
                                           Text = user.Name
                                       }).ToListAsync();
            return model;
        }

        public async Task<ApplicationUser> GetTrainerById(string id)
        {
            var trainer = await _db.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == id);
            return trainer;
        }
        public async Task<bool> isTrainerPercentageExist(string id)
        {
            var trainer = await _db.TrainerPercentages.FirstOrDefaultAsync(x => x.TrainerId == id);
            if (trainer != null)
            {
                return true;
            }
            return false;
        }
        public async Task AddTrainerPercentage(string TrainerId,int Percentage)
        {
            string sql = "INSERT INTO TrainerPercentages (percentage, TrainerId) VALUES (@Percentage, @TrainerId)";
            await _db.Database.ExecuteSqlRawAsync(sql,
                new SqlParameter("@Percentage", Percentage),
                new SqlParameter("@TrainerId", TrainerId));
        }
        public async Task UpdateTrainerPercentage(string TrainerId, int Percentage)
        {
            string sql = "Update TrainerPercentages SEt percentage = @Percentage Where TrainerId = @TrainerId";
            await _db.Database.ExecuteSqlRawAsync(sql,
                new SqlParameter("@Percentage", Percentage),
                new SqlParameter("@TrainerId", TrainerId));
        }
    }
}
