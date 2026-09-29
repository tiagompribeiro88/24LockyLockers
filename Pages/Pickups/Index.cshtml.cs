using _24LockyLockers.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace _24LockyLockers.Pages.Pickups;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "Access code is required")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Code must be 4 digits")]
        public string Code { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var locker = await _context.Lockers
            .FirstOrDefaultAsync(l => l.CurrentCode == Input.Code && !l.IsAvailable);

        if (locker == null)
        {
            ModelState.AddModelError("Input.Code", "Invalid code");
            return Page();
        }

        // Release locker
        locker.IsAvailable = true;
        locker.CurrentCode = null;
        locker.LastUsed = DateTime.Now;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Parcel collected from locker {locker.LockerNumber}!";
        return RedirectToPage("/Pickups/Index");
    }
}