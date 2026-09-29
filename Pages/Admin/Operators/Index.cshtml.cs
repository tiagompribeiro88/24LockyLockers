using _24LockyLockers.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OperatorModel = _24LockyLockers.Models.Operator;

namespace _24LockyLockers.Pages.Admin.Operators;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<OperatorModel> Operators { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        Operators = await _context.Operators
            .OrderBy(o => o.Name)
            .ToListAsync();

        return Page();
    }
}