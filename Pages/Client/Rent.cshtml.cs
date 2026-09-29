using _24LockyLockers.Data;
using _24LockyLockers.Models;
using _24LockyLockers.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace _24LockyLockers.Pages.Client;

/// <summary>
/// Page model for renting a locker.
/// </summary>
[Authorize(Roles = "Client")]
public class RentModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly ILogger<RentModel> _logger;

    public RentModel(
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager,
        IEmailService emailService,
        ILogger<RentModel> logger)
    {
        _context = context;
        _userManager = userManager;
        _emailService = emailService;
        _logger = logger;
    }

    public Locker? Locker { get; set; }
    public decimal Price { get; set; } = 5.00m;

    public async Task<IActionResult> OnGetAsync(int lockerId)
    {
        Locker = await _context.Lockers
            .FirstOrDefaultAsync(l => l.Id == lockerId && l.IsAvailable);

        if (Locker == null)
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int lockerId)
    {
        var locker = await _context.Lockers.FindAsync(lockerId);

        if (locker == null || !locker.IsAvailable)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        // Generate unique access code
        var accessCode = GenerateAccessCode();

        var rental = new Rental
        {
            UserId = userId,
            LockerId = lockerId,
            StartTime = DateTime.UtcNow,
            EndTime = null,
            Price = Price,
            AccessCode = accessCode,
            IsActive = true
        };

        _context.Rentals.Add(rental);
        locker.IsAvailable = false;
        locker.LastUsed = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Send confirmation email (non-blocking)
        await SendConfirmationEmailAsync(user, locker, accessCode);

        TempData["AccessCode"] = accessCode;
        TempData["LockerNumber"] = locker.LockerNumber;
        TempData["Price"] = Price.ToString("F2");

        return RedirectToPage("/Client/RentalConfirmation");
    }

    private static string GenerateAccessCode()
    {
        // Use cryptographically stronger random for production
        return new Random().Next(100000, 999999).ToString();
    }

    private async Task SendConfirmationEmailAsync(IdentityUser user, Locker locker, string accessCode)
    {
        if (string.IsNullOrEmpty(user.Email))
        {
            _logger.LogWarning("User {UserId} has no email address", user.Id);
            return;
        }

        var emailBody = CreateConfirmationEmailBody(locker, accessCode);

        try
        {
            await _emailService.SendEmailAsync(
                user.Email,
                "Rental Confirmation - 24LockyLockers",
                emailBody);

            _logger.LogInformation("Confirmation email sent to {Email}", user.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send confirmation email to {Email}", user.Email);
            // Don't fail the rental if email fails
        }
    }

    private static string CreateConfirmationEmailBody(Locker locker, string accessCode)
    {
        return $@"
            <html>
            <body>
                <h2>Rental Confirmed!</h2>
                <p>Thank you for renting a locker with 24LockyLockers.</p>
                <h3>Rental Details:</h3>
                <ul>
                    <li><strong>Locker:</strong> {locker.LockerNumber}</li>
                    <li><strong>Access Code:</strong> <span style='font-size: 24px; font-weight: bold;'>{accessCode}</span></li>
                    <li><strong>Price:</strong> €5.00</li>
                    <li><strong>Start Time:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC</li>
                </ul>
                <p>Use the access code above to unlock your locker.</p>
                <p>Best regards,<br/>24LockyLockers Team</p>
            </body>
            </html>";
    }
}