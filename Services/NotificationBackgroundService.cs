namespace _24LockyLockers.Services;

/// <summary>
/// Background service that checks for expiring rentals every hour.
/// </summary>
public class NotificationBackgroundService : BackgroundService
{
    private readonly ILogger<NotificationBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

    public NotificationBackgroundService(
        ILogger<NotificationBackgroundService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Notification background service starting");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Creates scope for each exec
                using var scope = _serviceProvider.CreateScope();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                await notificationService.CheckExpiringRentalsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking expiring rentals");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("Notification background service stopping");
    }
}