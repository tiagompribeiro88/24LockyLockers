using _24LockyLockers.Data;
using _24LockyLockers.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace _24LockyLockers.Pages.Admin.Lockers;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public List<Location> Locations { get; set; } = new();

    public class InputModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Locker number is required")]
        [StringLength(20, ErrorMessage = "Locker number cannot exceed 20 characters")]
        [RegularExpression(@"^[A-Z]{2}-[A-Z]{3}-\d{3}$",
            ErrorMessage = "Locker number must be in format XX-XXX-000 (e.g., PT-OPO-001)")]
        public string LockerNumber { get; set; } = string.Empty;

        public bool IsAvailable { get; set; } = true;

        [Required(ErrorMessage = "Location is required")]
        public int LocationId { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var locker = await _context.Lockers.FindAsync(id);
        if (locker == null)
        {
            return NotFound();
        }

        Input = new InputModel
        {
            Id = locker.Id,
            LockerNumber = locker.LockerNumber,
            IsAvailable = locker.IsAvailable,
            LocationId = locker.LocationId
        };

        Locations = await _context.Locations.OrderBy(l => l.LocationName).ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Locations = await _context.Locations.OrderBy(l => l.LocationName).ToListAsync();
            return Page();
        }

        var locker = await _context.Lockers.FindAsync(Input.Id);
        if (locker == null)
        {
            return NotFound();
        }

        // Check if locker number already exists (for another locker)
        var existingLocker = await _context.Lockers
            .FirstOrDefaultAsync(l => l.LockerNumber == Input.LockerNumber && l.Id != Input.Id);

        if (existingLocker != null)
        {
            ModelState.AddModelError("Input.LockerNumber", "Locker number already exists");
            Locations = await _context.Locations.OrderBy(l => l.LocationName).ToListAsync();
            return Page();
        }

        locker.LockerNumber = Input.LockerNumber;
        locker.IsAvailable = Input.IsAvailable;
        locker.LocationId = Input.LocationId;

        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}