using Gym_System.Controllers;
using Gym_System.Models;
using Gym_System.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography.Xml;

namespace Gym_System.Repository
{
    public class RegistraionRepo : IRegistraionRepo
    {
        private readonly ApplicationDbContext _db;

        public RegistraionRepo(ApplicationDbContext Db)
        {
            _db = Db;
        }
        public ApplicationUser RegisterUser(RegistrationUserModel newUser)
        {
            // Retrieve the membership from the database
            var membership = _db.Membrtships.FirstOrDefault(i => i.Id == newUser.MembrtshipsId);

            // Initialize the ApplicationUser object
            var userAccount = new ApplicationUser
            {
                Id = newUser.Id,
                UserName = newUser.UserName,
                PasswordHash = newUser.Password,
                Name = newUser.Name,
                Email = newUser.Email,
                PhoneNumber = newUser.PhoneNum,
                JoinDate = DateTime.Now,
                MembrtshipsId = membership != null ? newUser.MembrtshipsId : null,
                MembershipStartDate = newUser.MembershipStartDate,
                Balance = membership != null ? membership.Price : 0, // Default to 0 if membership is null
                AllowDays = newUser.AllowDays,
                TrainerId = newUser.TrainerId,
            };
            return userAccount;
        }
        public async Task<List<SelectListItem>> GetUserNameListAsync()
        {
            return await (from user in _db.ApplicationUsers
                          join userRole in _db.UserRoles on user.Id equals userRole.UserId
                          join role in _db.Roles on userRole.RoleId equals role.Id
                          where role.Name == "Client"
                          select new SelectListItem
                          {
                              Value = user.Id,
                              Text = user.Name
                          }).ToListAsync();
        }
        public async Task<List<SelectListItem>> GetUserNameAndTrainerListAsync()
        {
            return await (from user in _db.ApplicationUsers
                          join userRole in _db.UserRoles on user.Id equals userRole.UserId
                          join role in _db.Roles on userRole.RoleId equals role.Id
                          where role.Name == "Client" || role.Name =="Trainer"
                          select new SelectListItem
                          {
                              Value = user.Id,
                              Text = user.Name
                          }).ToListAsync();
        }
        public async Task<List<SelectListItem>> GetTrainerListNameAsync()
        {
            return await (from user in _db.ApplicationUsers
                          join userRole in _db.UserRoles on user.Id equals userRole.UserId
                          join role in _db.Roles on userRole.RoleId equals role.Id
                          where role.Name == "Trainer"
                          select new SelectListItem
                          {
                              Value = user.Id,
                              Text = user.Name
                          }).ToListAsync();
        }

        public async Task updateuser(UpdateUserViewModel model)
        {
            var user = await _db.ApplicationUsers.FirstOrDefaultAsync(u=>u.Id==model.Id);
            // Update user properties
            user.Name = model.Name;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNum;
            user.UserName = model.UserName;
            user.AllowDays = model.AllowDays;
            user.Balance = model.Balance;
            user.JoinDate = model.JoinDate;
            user.MembershipStartDate = model.MembershipStartDate;
            user.TrainerId = model.TarinerId;
            await UpdateDiscountAsync(model.Id,model.Discount);
            await _db.SaveChangesAsync();

        }
        public async Task<List<SelectListItem>> GetTrainersAsync()
        {
            var trainers = await (from user in _db.ApplicationUsers
                                  join userRole in _db.UserRoles on user.Id equals userRole.UserId
                                  join role in _db.Roles on userRole.RoleId equals role.Id
                                  where role.Name == "Trainer"
                                  select new SelectListItem
                                  {
                                      Value = user.Id.ToString(),
                                      Text = user.Name
                                  }).ToListAsync();

            return trainers;
        }
        public async Task<List<SelectListItem>> GetTrainersAndSubAdminAsync()
        {
            var trainers = await (from user in _db.ApplicationUsers
                                  join userRole in _db.UserRoles on user.Id equals userRole.UserId
                                  join role in _db.Roles on userRole.RoleId equals role.Id
                                  where role.Name == "Trainer" || role.Name == "SubAdmin"
                                  select new SelectListItem
                                  {
                                      Value = user.Id.ToString(),
                                      Text = user.Name
                                  }).ToListAsync();

            return trainers;
        }
        public async Task<ApplicationUser> GetUserById(string id)
        {
            var user = await _db.Users.Include(i=>i.Membrtships).Include(s=>s.Trainer).FirstOrDefaultAsync(u=>u.Id==id);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID '{id}' was not found.");
            }
            return user;
        }
        public async Task<ApplicationUser> GetUserByUserName(string name)
        {
            var user = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName == name);
            return user;
        }
        public async Task<ApplicationUser> GetUserByPhone(string phone)
        {
            var user = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.PhoneNumber == phone);
            return user;
        }
        public async Task<ApplicationUser> GetUserByName(string name)
        {
            var user = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.Name == name);
            return user;
        }
        public async Task<ApplicationUser> GetUserByEmail(string email)
        {
            var user = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }
        public async Task<ClintDiscount> GetClientDiscount(string id)
        {
            var Discount = await _db.ClintDiscounts.FirstOrDefaultAsync(i=>i.ClintId==id);
            return Discount;
        }



        public RegistrationUserModel GetMemberShipAndTrainerList()
        {
            var model = new RegistrationUserModel();

                model.MembershipsList = _db.Membrtships
                    .Select(m => new SelectListItem
                    {
                        Value = m.Id.ToString(),  // Store the Membership ID
                        Text = m.Name             // Display the Membership Name
                    }).ToList();
                model.TrainerList = (from user in _db.ApplicationUsers
                                          join userRole in _db.UserRoles on user.Id equals userRole.UserId
                                          join role in _db.Roles on userRole.RoleId equals role.Id
                                          where role.Name == "Trainer"
                                          select new SelectListItem
                                          {
                                              Value = user.Id.ToString(),
                                              Text = user.Name
                                          }).ToList();
            return model;
        }
        public async Task AddDiscount(string id, decimal discount)
        {
            string sql = "INSERT INTO ClintDiscounts (ClintId, Discount) VALUES (@ClintId, @Discount)";
            await _db.Database.ExecuteSqlRawAsync(sql,
                new SqlParameter("@ClintId", id),
                new SqlParameter("@Discount", discount));
        }
        public async Task UpdateDiscountAsync(string id, decimal discount)
        {
            if (string.IsNullOrEmpty(id))
            {
                Console.WriteLine("Invalid Client ID");
                return;
            }

            // Check if the client exists
            var user = await _db.ClintDiscounts
                .FirstOrDefaultAsync(i => EF.Functions.Like(i.ClintId.Trim(), id.Trim()));

            if (user == null)
            {
                // Insert new record if ClintId does not exist
                string insertQuery = "INSERT INTO ClintDiscounts (ClintId, Discount) VALUES (@ClintId, @Discount)";
                await _db.Database.ExecuteSqlRawAsync(insertQuery,
                    new SqlParameter("@ClintId", id),
                    new SqlParameter("@Discount", discount));

                Console.WriteLine("New discount record inserted.");
            }
            else
            {
                // Update existing record
                string updateQuery = "UPDATE ClintDiscounts SET Discount = @Discount WHERE ClintId = @ClintId";
                await _db.Database.ExecuteSqlRawAsync(updateQuery,
                    new SqlParameter("@ClintId", id),
                    new SqlParameter("@Discount", discount));

                Console.WriteLine("Discount updated successfully.");
            }
        }

        public async Task<MembershipVM> GetMemberShipListForMembership()
        {
            var model = new MembershipVM();

            model.MembershipsList =await _db.Membrtships
                .Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),  // Store the Membership ID
                    Text = m.Name             // Display the Membership Name
                }).ToListAsync();
            return model;
        }
        public async Task<RenewMembershipVM> GetMemberShipListUserList()
        {
            var model = new RenewMembershipVM();
            model.Membership = await _db.Membrtships
                .Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),  // Store the Membership ID
                    Text = m.Name             // Display the Membership Name
                }).ToListAsync();
            model.User = await (from user in _db.ApplicationUsers
                                join userRole in _db.UserRoles on user.Id equals userRole.UserId
                                join role in _db.Roles on userRole.RoleId equals role.Id
                                where role.Name == "Client"
                                select new SelectListItem
                                {
                                    Value = user.Id,
                                    Text = user.Name
                                }).ToListAsync();
            return model;
        }

        public async Task<List<SelectListItem>> GetMemberShipList()
        {
            return await _db.Membrtships.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Name
            }).ToListAsync();

        }
        public async Task RenewMembership(string userid, int membershipid,int AllowDays)
        {
            var user =await _db.ApplicationUsers.FirstOrDefaultAsync(i=>i.Id == userid);
            var membership = await _db.Membrtships.FirstOrDefaultAsync(m=>m.Id == membershipid);

            var balance = user.Balance;
            var membershipprice = membership.Price;
            user.Balance = balance+membershipprice;
            user.MembrtshipsId= membership.Id;
            user.MembershipStartDate=DateTime.Now;
            user.AllowDays = AllowDays;
            _db.SaveChanges();
        }

        public async Task<Membrtship> GetMemberShipByID(int Id)
        {
            var membership =await _db.Membrtships
                .Where(m => m.Id == Id)
                .FirstOrDefaultAsync();
            return membership;
        }

        public async Task UpdateMemberShip(int Id , MembershipVM model)
        {
            var membership = await _db.Membrtships.FirstOrDefaultAsync(m => m.Id == Id);
            if (membership != null)
            {
                // Update properties
                membership.Price = model.Price;
                membership.Name = model.Name;
                membership.DurationInDays = model.DurationInDays;
                membership.MaxVisits = model.MaxVisits;
                // Save changes to the database
                await _db.SaveChangesAsync();
            }
            else
            {
                throw new Exception($"Membership with Id {Id} not found.");
            }
        }

        public async Task DeleteMemberShip(int Id)
        {
            var membership =await _db.Membrtships.FirstOrDefaultAsync
                (m => m.Id == Id);
            if (membership != null)
            {
                _db.Membrtships.Remove(membership);
                _db.SaveChanges();
            }
        }

        public async Task<Membrtship> CreateMemberShip(MembershipVM Model)
        {
            Membrtship membrtship =
                new Membrtship
                {
                    Name = Model.Name,
                    DurationInDays = Model.DurationInDays,
                    MaxVisits = Model.MaxVisits,
                    Price = Model.Price
                };
            await _db.Membrtships.AddAsync(membrtship);
            _db.SaveChanges();
            return membrtship;
        }
    }
}
