using Gym_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class LinkController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public LinkController(ApplicationDbContext context,
                          UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET
    public async Task<IActionResult> Create()
    {
        // Get linked user ids
        var linkedUserIds = await _context.Links
            .Select(x => x.UserId)
            .ToListAsync();

        // Get users NOT linked
        var users = await _context.Users
            .Where(u => !linkedUserIds.Contains(u.Id))
            .ToListAsync();

        ViewBag.Users = new SelectList(users, "Id", "Name");

        return View();
    }

    // POST
    [HttpPost]
    public async Task<IActionResult> Create(Link model)
    {
        if (ModelState.IsValid)
        {
            // check if already linked
            var exists = await _context.Links
                .AnyAsync(x => x.UserId == model.UserId);

            if (exists)
            {
                ModelState.AddModelError("", "This user already linked");
            }
            else
            {
                _context.Links.Add(model);
                await _context.SaveChangesAsync();

                return RedirectToAction("Create");
            }
        }

        // reload users
        var linkedUserIds = await _context.Links
            .Select(x => x.UserId)
            .ToListAsync();

        var users = await _context.Users
            .Where(u => !linkedUserIds.Contains(u.Id))
            .ToListAsync();

        ViewBag.Users = new SelectList(users, "Id", "UserName");

        return View(model);
    }
}
