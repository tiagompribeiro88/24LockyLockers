using _24LockyLockers.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace _24LockyLockers.Pages.Lockers;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Models.Locker> Lockers { get; set; } = new();
    public string Location { get; set; } = "";
    public string LocationClass { get; set; } = "";
    public string DisplayName { get; set; } = "";

    public async Task<IActionResult> OnGetAsync(string? location)
    {
        Location = location ?? "";

        // Get location class and display name
        (LocationClass, DisplayName) = GetLocationInfo(location);

        // Get lockers
        Lockers = await GetLockersByLocation(location);

        return Page();
    }

    private static (string locationClass, string displayName) GetLocationInfo(string? location)
    {
        if (string.IsNullOrEmpty(location))
            return ("", "All Lockers");

        return location switch
        {
            var loc when loc.Contains("PT-OPO") || loc.Contains("OPO") => ("locker-pt-opo", "PORTO"),
            var loc when loc.Contains("PT-LIS") || loc.Contains("LIS") => ("locker-pt-lis", "LISBOA"),
            var loc when loc.Contains("PT-CPB") || loc.Contains("CPB") => ("locker-pt-cpb", "COIMBRA"),
            var loc when loc.Contains("ES-BCN") || loc.Contains("BCN") => ("locker-es-bcn", "BARCELONA"),
            var loc when loc.Contains("ES-MAD") || loc.Contains("MAD") => ("locker-es-mad", "MADRID"),
            var loc when loc.Contains("ES-SCQ") || loc.Contains("SCQ") => ("locker-es-scq", "SANTIAGO DE COMPOSTELA"),
            _ => ("", location)
        };
    }

    private async Task<List<Models.Locker>> GetLockersByLocation(string? location)
    {
        if (string.IsNullOrEmpty(location))
        {
            return await _context.Lockers
                .OrderBy(l => l.LockerNumber)
                .ToListAsync();
        }

        return await _context.Lockers
            .Where(l => l.LockerNumber.StartsWith(location))
            .OrderBy(l => l.LockerNumber)
            .ToListAsync();
    }
}