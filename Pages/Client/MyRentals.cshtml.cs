using _24LockyLockers.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace _24LockyLockers.Pages.Client;

/// <summary>
/// Page model for viewing user's rentals.
/// </summary>
[Authorize(Roles = "Client")]
public class MyRentalsModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public MyRentalsModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public List<Models.Rental> ActiveRentals { get; set; } = new();
    public List<Models.Rental> PastRentals { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        // Get active rentals
        ActiveRentals = await _context.Rentals
            .Include(r => r.Locker)
            .ThenInclude(l => l.Location)
            .Where(r => r.UserId == userId && r.IsActive)
            .OrderByDescending(r => r.StartTime)
            .ToListAsync();

        // Get past rentals
        PastRentals = await _context.Rentals
            .Include(r => r.Locker)
            .ThenInclude(l => l.Location)
            .Where(r => r.UserId == userId && !r.IsActive)
            .OrderByDescending(r => r.StartTime)
            .Take(10)
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostEndRentalAsync(int rentalId)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var rental = await _context.Rentals
            .Include(r => r.Locker)
            .FirstOrDefaultAsync(r => r.Id == rentalId && r.UserId == userId && r.IsActive);

        if (rental == null || rental.Locker == null)
        {
            return NotFound();
        }

        // End rental
        rental.EndTime = DateTime.UtcNow;
        rental.IsActive = false;

        // Mark locker as available again
        rental.Locker.IsAvailable = true;

        await _context.SaveChangesAsync();

        TempData["Success"] = "Rental ended successfully.";
        return RedirectToPage("/Client/MyRentals");
    }
}