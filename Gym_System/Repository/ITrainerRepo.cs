using Gym_System.Models;
using Gym_System.ViewModel;

namespace Gym_System.Repository
{
    public interface ITrainerRepo
    {
        public Task<TrainerVM> GetTrainerList();
        public Task<ApplicationUser> GetTrainerById(string id);
        public Task AddTrainerPercentage(string id, int  percentage);   
        public Task UpdateTrainerPercentage(string id, int  percentage);
        public Task<bool> isTrainerPercentageExist(string id);

    }
}
