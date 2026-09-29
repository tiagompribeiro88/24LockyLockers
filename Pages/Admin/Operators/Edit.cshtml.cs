using _24LockyLockers.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace _24LockyLockers.Pages.Admin.Operators;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var op = await _context.Operators.FindAsync(id);
        if (op == null)
        {
            return NotFound();
        }

        Input = new InputModel
        {
            Id = op.Id,
            Name = op.Name,
            Email = op.Email,
            IsActive = op.IsActive
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var op = await _context.Operators.FindAsync(Input.Id);
        if (op == null)
        {
            return NotFound();
        }

        // Check if email already exists (for another operator)
        var existingOp = await _context.Operators
            .FirstOrDefaultAsync(o => o.Email == Input.Email && o.Id != Input.Id);

        if (existingOp != null)
        {
            ModelState.AddModelError("Input.Email", "Email already exists");
            return Page();
        }

        op.Name = Input.Name;
        op.Email = Input.Email;
        op.IsActive = Input.IsActive;

        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}