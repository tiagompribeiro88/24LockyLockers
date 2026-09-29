using _24LockyLockers.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace _24LockyLockers.Services;

/// <summary>
/// Service for sending rental expiration notifications.
/// </summary>
public interface INotificationService
{
    Task CheckExpiringRentalsAsync();
}

/// <summary>
/// Implementation of notification service.
/// </summary>
public class NotificationService : INotificationService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<NotificationService> _logger;
    private readonly IEmailService _emailService;

    public NotificationService(
        IServiceScopeFactory scopeFactory,
        ILogger<NotificationService> logger,
        IEmailService emailService)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _emailService = emailService;
    }

    public async Task CheckExpiringRentalsAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        var expiringThreshold = DateTime.UtcNow.AddHours(24); // Reservas com menos de 24h

        var expiringRentals = await context.Rentals
            .Include(r => r.Locker)
            .Include(r => r.User)
            .Where(r => r.IsActive && r.StartTime < expiringThreshold)
            .ToListAsync();

        foreach (var rental in expiringRentals)
        {
            // fixes null checks
            if (rental.User?.Email == null || rental.Locker == null)
            {
                continue;
            }

            var hoursSinceStart = (DateTime.UtcNow - rental.StartTime).TotalHours;
            var hoursRemaining = 24 - hoursSinceStart;

            // Notifies remaining time
            if (hoursRemaining <= 2 && hoursRemaining > 0)
            {
                var emailBody = $@"
                    <html>
                    <body>
                        <h2>Rental Expiring Soon!</h2>
                        <p>Your locker rental is about to expire.</p>
                        <h3>Rental Details:</h3>
                        <ul>
                            <li><strong>Locker:</strong> {rental.Locker.LockerNumber}</li>
                            <li><strong>Time Remaining:</strong> {hoursRemaining:F1} hours</li>
                        </ul>
                        <p>Please return the locker before the time expires.</p>
                        <p>Best regards,<br/>24LockyLockers Team</p>
                    </body>
                    </html>
                ";

                try
                {
                    await _emailService.SendEmailAsync(
                        rental.User.Email,
                        "Rental Expiring Soon - 24LockyLockers",
                        emailBody);

                    _logger.LogInformation("Sent expiration notification to {Email}", rental.User.Email);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send notification to {Email}", rental.User.Email);
                }
            }
        }
    }
}