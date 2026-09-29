using _24LockyLockers.Data;
using _24LockyLockers.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace _24LockyLockers.Pages.Admin.Lockers;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public List<Location> Locations { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "Locker number is required")]
        [StringLength(20, ErrorMessage = "Locker number cannot exceed 20 characters")]
        [RegularExpression(@"^[A-Z]{2}-[A-Z]{3}-\d{3}$",
            ErrorMessage = "Locker number must be in format XX-XXX-000 (e.g., PT-OPO-001)")]
        public string LockerNumber { get; set; } = string.Empty;

        public bool IsAvailable { get; set; } = true;

        [Required(ErrorMessage = "Location is required")]
        public int LocationId { get; set; }
    }

    public IActionResult OnGet()
    {
        Locations = _context.Locations.OrderBy(l => l.LocationName).ToList();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Locations = _context.Locations.OrderBy(l => l.LocationName).ToList();
            return Page();
        }

        // Check if locker number already exists
        var existingLocker = await _context.Lockers
            .FirstOrDefaultAsync(l => l.LockerNumber == Input.LockerNumber);

        if (existingLocker != null)
        {
            ModelState.AddModelError("Input.LockerNumber", "Locker number already exists");
            Locations = _context.Locations.OrderBy(l => l.LocationName).ToList();
            return Page();
        }

        var locker = new Locker
        {
            LockerNumber = Input.LockerNumber,
            IsAvailable = Input.IsAvailable,
            LocationId = Input.LocationId
        };

        _context.Lockers.Add(locker);
        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}