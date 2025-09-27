using Gym_System.Repository;
using Gym_System.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gym_System.Controllers
{
    [Authorize]
    public class MembershipController : Controller
    {
        private readonly IRegistraionRepo _userservices;
        public MembershipController(IRegistraionRepo Userservices)
        {
            _userservices = Userservices;
        }
        //------------------------- MemberShips --------------------------------
        [HttpGet]
        public async Task<IActionResult> GetMemberships()
        {
            var model = await _userservices.GetMemberShipListForMembership();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetMembershipDetails(int id)
        {
            var membership = await _userservices.GetMemberShipByID(id);

            if (membership == null)
            {
                return NotFound();
            }
            return Json(membership);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> CreateMembership(MembershipVM membership)
        {
            if (!ModelState.IsValid)
            {
                // Return the same view with validation errors
                return RedirectToAction("GetMemberships", membership);
            }
            try
            {
                await _userservices.CreateMemberShip(membership);

                // Display a success message using TempData
                TempData["SuccessMessage"] = "Membership created successfully.";
                return RedirectToAction("GetMemberships");
            }
            catch (ArgumentException argEx)
            {
                // Handle argument-related exceptions
                ModelState.AddModelError(string.Empty, argEx.Message);
            }
            catch (InvalidOperationException invOpEx)
            {
                // Handle invalid operations (e.g., duplicates)
                ModelState.AddModelError(string.Empty, "Operation failed: " + invOpEx.Message);
            }
            catch (Exception ex)
            {
                // Display a generic error message
                ModelState.AddModelError(ex.Message, "An unexpected error occurred. Please try again later.");
            }

            // Return the view with the model to preserve data
            return RedirectToAction("GetMemberships");

        }
        [HttpPost]
        public async Task<IActionResult> UpdateMembership(int Id, MembershipVM newmodel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("GetMemberships", newmodel);
            }
            try
            {
                await _userservices.UpdateMemberShip(Id, newmodel);
                return RedirectToAction("GetMemberships");
            }
            catch (ArgumentException argEx)
            {
                // Handle argument-related exceptions
                ModelState.AddModelError(string.Empty, argEx.Message);
            }
            catch (InvalidOperationException invOpEx)
            {
                // Handle invalid operations (e.g., duplicates)
                ModelState.AddModelError(string.Empty, "Operation failed: " + invOpEx.Message);
            }
            catch (Exception ex)
            {
                // Display a generic error message
                ModelState.AddModelError(ex.Message, "An unexpected error occurred. Please try again later.");
            }

            // Return the view with the model to preserve data
            return RedirectToAction("GetMemberships");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMembership(int Id)
        {
            try
            {
                await _userservices.DeleteMemberShip(Id);
                return RedirectToAction("GetMemberships");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("MembershipsList", "You can't delete this membership.");
                var model = await _userservices.GetMemberShipListForMembership();

                return View("GetMemberships",model);
            }

        }

        [HttpGet]
        public async Task<IActionResult> RenewMembership()
        {
            var model =await _userservices.GetMemberShipListUserList();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> RenewMembershippost(RenewMembershipVM model)
        {
            if (ModelState.IsValid)
            {
                await _userservices.RenewMembership(model);
                if(model.Discount>=0)
                {
                    await _userservices.UpdateDiscountAsync(model.SelectedUserId, model.Discount);
                }
            }
            var model2 = await _userservices.GetMemberShipListForMembership();

            return RedirectToAction("RenewMembership");
        }

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
                allowDays = user.AllowDays,
                membershipStartDate = user.MembershipStartDate.AddDays(user.Membrtships?.DurationInDays ?? 0).AddDays(user.Freezes?.FreezeDays ?? 0).ToString("yyyy-MM-dd"),
                discount = Discountuser,
                membershipid = user.MembrtshipsId
            });
        }
    }
}
