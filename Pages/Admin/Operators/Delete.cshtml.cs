using _24LockyLockers.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OperatorModel = _24LockyLockers.Models.Operator;

namespace _24LockyLockers.Pages.Admin.Operators;

public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public OperatorModel? Operator { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Operator = await _context.Operators.FindAsync(id);
        if (Operator == null)
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var op = await _context.Operators.FindAsync(id);
        if (op == null)
        {
            return NotFound();
        }

        _context.Operators.Remove(op);
        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}