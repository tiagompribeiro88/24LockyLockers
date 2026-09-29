using _24LockyLockers.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace _24LockyLockers.Pages.Client.Lockers
{
    [Authorize(Roles = "Client")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<_24LockyLockers.Models.Locker> Lockers { get; set; } = new();
        public string Location { get; set; } = "";

        public async Task<IActionResult> OnGetAsync(string? location)
        {
            Location = location ?? "";

            if (!string.IsNullOrEmpty(location))
            {
                Lockers = await _context.Lockers
                    .Where(l => l.LockerNumber.StartsWith(location))
                    .OrderBy(l => l.LockerNumber)
                    .ToListAsync();
            }
            else
            {
                Lockers = await _context.Lockers
                    .Where(l => l.IsAvailable)
                    .OrderBy(l => l.LockerNumber)
                    .ToListAsync();
            }

            return Page();
        }
    }
}