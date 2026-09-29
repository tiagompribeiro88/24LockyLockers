using _24LockyLockers.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace _24LockyLockers.Pages.Deposits;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public List<SelectListItem> AvailableLockers { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "Please select a locker")]
        public string LockerNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Recipient name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string RecipientName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Recipient email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string RecipientEmail { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var availableLockers = await _context.Lockers
            .Where(l => l.IsAvailable)
            .OrderBy(l => l.LockerNumber)
            .ToListAsync();

        AvailableLockers = availableLockers
            .Select(l => new SelectListItem
            {
                Value = l.LockerNumber,
                Text = l.LockerNumber
            })
            .ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var locker = await _context.Lockers
            .FirstOrDefaultAsync(l => l.LockerNumber == Input.LockerNumber && l.IsAvailable);

        if (locker == null)
        {
            ModelState.AddModelError("Input.LockerNumber", "Locker not available");
            return Page();
        }

        // Generate unique code
        var code = GenerateUniqueCode();

        locker.IsAvailable = false;
        locker.CurrentCode = code;
        locker.LastUsed = DateTime.Now;

        await _context.SaveChangesAsync();

        // In a real app, send email with code to recipient

        TempData["SuccessMessage"] = $"Parcel deposited! Code: {code}";
        return RedirectToPage("/Deposits/Index");
    }

    private string GenerateUniqueCode()
    {
        return new Random().Next(1000, 9999).ToString();
    }
}