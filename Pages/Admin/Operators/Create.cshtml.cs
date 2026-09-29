using _24LockyLockers.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using OperatorModel = _24LockyLockers.Models.Operator;

namespace _24LockyLockers.Pages.Admin.Operators;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
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

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Check if email already exists
        var existingOperator = await _context.Operators
            .FirstOrDefaultAsync(o => o.Email == Input.Email);

        if (existingOperator != null)
        {
            ModelState.AddModelError("Input.Email", "Email already exists");
            return Page();
        }

        var operatorEntity = new OperatorModel
        {
            Name = Input.Name,
            Email = Input.Email,
            IsActive = Input.IsActive,
            CreatedAt = DateTime.Now
        };

        _context.Operators.Add(operatorEntity);
        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}