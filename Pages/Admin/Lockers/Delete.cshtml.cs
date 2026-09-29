using _24LockyLockers.Data;
using _24LockyLockers.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace _24LockyLockers.Pages.Admin.Lockers;

public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Locker? Locker { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Locker = await _context.Lockers
            .Include(l => l.Location)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (Locker == null)
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var locker = await _context.Lockers.FindAsync(id);
        if (locker == null)
        {
            return NotFound();
        }

        _context.Lockers.Remove(locker);
        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}