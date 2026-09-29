using _24LockyLockers.Data;
using _24LockyLockers.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace _24LockyLockers.Pages.Admin.Lockers;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Locker> Lockers { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        Lockers = await _context.Lockers
            .OrderBy(l => l.LockerNumber)
            .ToListAsync();

        return Page();
    }
}