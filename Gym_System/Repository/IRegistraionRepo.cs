using Gym_System.Models;
using Gym_System.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gym_System.Repository
{
    public interface IRegistraionRepo
    {
        public ApplicationUser RegisterUser(RegistrationUserModel item);
        public RegistrationUserModel GetMemberShipAndTrainerList();

        public Task AddDiscount(string id, decimal discount);
        public Task UpdateDiscountAsync(string id, decimal discount);
        public Task<ApplicationUser> GetUserById(string id);
        public Task<ApplicationUser> GetUserByUserName(string name);
        public Task<ApplicationUser> GetUserByPhone(string phone);
        public Task<ApplicationUser> GetUserByName(string name);
        public Task<ApplicationUser> GetUserByEmail(string name);
        public Task updateuser(UpdateUserViewModel item);
        public Task<List<SelectListItem>> GetUserNameListAsync();
        public Task<List<SelectListItem>> GetUserNameAndTrainerListAsync();
        public Task<List<SelectListItem>> GetTrainerListNameAsync();
        public Task<List<SelectListItem>> GetMemberShipList();
        public Task<List<SelectListItem>> GetTrainersAndSubAdminAsync();
        public Task<List<SelectListItem>> GetTrainersAsync();
        public Task<ClintDiscount> GetClientDiscount(string id);
        public Task<MembershipVM> GetMemberShipListForMembership();
        public Task<RenewMembershipVM> GetMemberShipListUserList();
        public Task RenewMembership(string userid, int membershipid, int allowdays);
        public Task<Membrtship> GetMemberShipByID(int Id);
        public Task UpdateMemberShip(int Id,MembershipVM model);
        public Task DeleteMemberShip(int Id);
        public Task<Membrtship> CreateMemberShip(MembershipVM Model);

    }
}