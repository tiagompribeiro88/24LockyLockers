using System.Net;
using System.Net.Mail;

using System.Net;
using System.Net.Mail;

namespace _24LockyLockers.Services;

/// <summary>
/// Service for sending emails via SMTP.
/// </summary>
public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body);
}

/// <summary>
/// Implementation of email service using SMTP.
/// </summary>
public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        // Get settings with null checks
        var smtpServer = _config["EmailSettings:SmtpServer"];
        var smtpPortStr = _config["EmailSettings:SmtpPort"] ?? "587";
        var senderEmail = _config["EmailSettings:SenderEmail"];
        var senderName = _config["EmailSettings:SenderName"];
        var username = _config["EmailSettings:Username"];
        var password = _config["EmailSettings:Password"];

        // Validate required settings
        if (string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(senderEmail))
        {
            _logger.LogWarning("Email settings not configured. Email not sent.");
            return; // Fail silently instead of crashing
        }

        // Parse port safely
        if (!int.TryParse(smtpPortStr, out var smtpPort))
        {
            smtpPort = 587; // Default fallback
        }

        var message = new MailMessage
        {
            From = new MailAddress(senderEmail, senderName ?? "24LockyLockers"),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        message.To.Add(toEmail);

        using var client = new SmtpClient(smtpServer, smtpPort)
        {
            Credentials = new NetworkCredential(username, password),
            EnableSsl = true
        };

        try
        {
            await client.SendMailAsync(message);
            _logger.LogInformation("Email sent to {ToEmail}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {ToEmail}", toEmail);
            // Don't rethrow - email is not critical for the app to work
        }
    }
}