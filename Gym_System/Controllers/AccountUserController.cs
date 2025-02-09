using Gym_System.Models;
using Gym_System.Repository;
using Gym_System.ViewModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Gym_System.Controllers
{
    public class AccountUser : Controller
    {
        private readonly IRegistraionRepo _userservices;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signIn;
        private readonly ApplicationDbContext _db;
        private readonly ITrainerRepo _trainerServece;


        public AccountUser(ITrainerRepo trainerServece,IRegistraionRepo Userservices,ApplicationDbContext Db, SignInManager<ApplicationUser> SignIn, UserManager<ApplicationUser> userManager)
        {
            _userservices = Userservices;
            _signIn = SignIn;
            _userManager = userManager;
            _db = Db;
            _trainerServece = trainerServece;
        }

        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //redirect to index
        public async Task<IActionResult> LogIn(LogInVM user)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the user account
                ApplicationUser account = await _userManager.FindByNameAsync(user.UserName);

                if (account != null)
                {
                    // Check if the security stamp is missing and update it if necessary
                    // Check if the password is correct
                    bool found = await _userManager.CheckPasswordAsync(account, user.Password);
                    if (found)
                    {
                        // Retrieve the user's roles
                        var roles = await _userManager.GetRolesAsync(account);

                        // Create a list of claims, adding roles as claims
                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, account.Id)
                };
                        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                        // Create a claims identity with these claims
                        var claimsIdentity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);

                        // Sign in using ASP.NET Core Identity's sign-in manager
                        await _signIn.SignInWithClaimsAsync(account, user.RememberMe, claims);

                        return RedirectToAction("Index","Home");
                    }
                }

                ModelState.AddModelError("", "User name or Password invalid");
            }
            return View(user);
        }

        [Authorize(Roles = "Admin,SubAdmin")]
        [HttpGet]
        public IActionResult RegisterUser(string id)
        {
            var model = _userservices.GetMemberShipAndTrainerList();
            // Pass the ID to the view via ViewBag or ViewModel
            ViewBag.UserId = id;

            return View(model);
        }

        [Authorize(Roles = "Admin,SubAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterUser(RegistrationUserModel newUser)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userservices.GetUserByUserName(newUser.UserName);
                newUser.MembershipsList = await _userservices.GetMemberShipList();
                newUser.TrainerList =await _userservices.GetTrainerListNameAsync();

                if (existingUser != null)
                {
                    ModelState.AddModelError("UserName", "Username already exists! Please choose another one.");
                    return View(newUser); // Stay on the registration page and show the error
                }
                var userbyemail = await _userservices.GetUserByEmail(newUser.Email);
                if (userbyemail != null && !string.IsNullOrEmpty(userbyemail.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists! Please choose another one.");
                    return View(newUser); // Stay on the registration page and show the error
                }
                var userbyphhone = await _userservices.GetUserByPhone(newUser.PhoneNum);
                if (userbyphhone != null)
                {
                    ModelState.AddModelError("PhoneNum", "Phone already exists! Please choose another one.");
                    return View(newUser); // Stay on the registration page and show the error
                }
                var userbyname = await _userservices.GetUserByName(newUser.Name);
                if (userbyname != null)
                {
                    ModelState.AddModelError("Name", "Username already exists! Please choose another one.");
                    return View(newUser); // Stay on the registration page and show the error
                }
                // Step 1: Map the ApplicationUser
                ApplicationUser userAccount = _userservices.RegisterUser(newUser);

                // Step 2: Save the user using UserManager
                var result = await _userManager.CreateAsync(userAccount, newUser.Password);

                if (result.Succeeded)
                {
                    // Step 3: Assign roles if provided
                    if (!string.IsNullOrEmpty(newUser.Role))
                    {
                        var validRoles = new[] { "SubAdmin", "Trainer", "Client" };
                        var TrainerOrSubAdmin = new[] { "SubAdmin", "Trainer" };
                        if (!validRoles.Contains(newUser.Role))
                        {
                            ModelState.AddModelError("", "Invalid role selected.");
                            return View(newUser);
                        }
                        if (TrainerOrSubAdmin.Contains(newUser.Role))
                        {
                            userAccount.MembershipState = "Active";
                        }
                        
                        await _userManager.AddToRoleAsync(userAccount, newUser.Role);
                        if(newUser.Discount>0  )
                        {
                            await _userservices.AddDiscount(newUser.Id, newUser.Discount);
                        }
                    }
                    if(newUser.Role== "Trainer")
                    {
                        return RedirectToAction("AddTrainerPercentage", "Trainer");
                    }
                    else if(newUser.Role== "SubAdmin")
                    {
                        return RedirectToAction("RegisterUser");
                    }
                    // Step 5: Redirect to AddTransaction
                    return RedirectToAction("AddTransaction", "Transaction");
                }
                else
                {
                    // Handle user creation errors
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return View(newUser);
        }


        [HttpGet]
        //[Authorize (Roles ="Admin")]
        public async new Task<IActionResult> SignOut()
        {
            await _signIn.SignOutAsync();
            return RedirectToAction("LogIn");
        }
        [HttpGet]

        [HttpGet]
        public async Task<IActionResult> UpdateUser()
        {
            // Populate users for the dropdown
            ViewBag.Users = await _userservices.GetUserNameListAsync();
            UpdateUserViewModel model = new UpdateUserViewModel();
            model.TrainerNames=await _userservices.GetTrainersAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserpost(UpdateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Reload dropdown list in case of error
                ViewBag.Users = await _userservices.GetUserNameListAsync();
                return View("UpdateUser", model);
            }

            try
            {
                await _userservices.updateuser(model);
                ViewBag.Users = await _userservices.GetUserNameListAsync();
                return RedirectToAction("UpdateUser");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred: " + ex.Message);
            }

            // Reload dropdown list in case of error
            ViewBag.Users = await _userservices.GetUserNameListAsync();
            return View("UpdateUser", model);
        }


        //json
        [HttpGet]
        public async Task<IActionResult> GetUserDetails(string id)
        {
            var user = await _userservices.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            var Discount = await _userservices.GetClientDiscount(id);
            decimal Discountuser = 0;

            if (Discount == null)
            {
                Discountuser = 0;
            }
            else
            {
                Discountuser = Discount.Discount;
            }

            return Json(new
            {
                id = user.Id,
                name = user.Name,
                email = user.Email,
                phoneNum = user.PhoneNumber,
                userName = user.UserName,
                allowDays = user.AllowDays,
                balance = user.Balance,
                joinDate = user.JoinDate.ToString("yyyy-MM-dd"),
                membershipStartDate = user.MembershipStartDate.ToString("yyyy-MM-dd"),
                discount = Discountuser,
                trainerid = user.TrainerId
            });
        }


    }
}