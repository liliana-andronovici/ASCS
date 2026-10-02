using InfigoHR.EmailService.Configuration;
using InfigoHR.EmailService.Models;

namespace InfigoHR.EmailService.Validation;

public static class EmailServiceValidator
{
    public static void ValidateSettings(SmtpSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        if (string.IsNullOrWhiteSpace(settings.Host))
        {
            throw new ArgumentException("SMTP host is required.", nameof(settings));
        }

        if (settings.Port is <= 0 or > 65535)
        {
            throw new ArgumentOutOfRangeException(nameof(settings), "SMTP port is invalid.");
        }

        if (string.IsNullOrWhiteSpace(settings.FromAddress))
        {
            throw new ArgumentException("From address is required.", nameof(settings));
        }
    }

    public static void ValidateMessage(EmailMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);

        if (string.IsNullOrWhiteSpace(message.To))
        {
            throw new ArgumentException("Recipient address is required.", nameof(message));
        }

        if (string.IsNullOrWhiteSpace(message.Subject))
        {
            throw new ArgumentException("Subject is required.", nameof(message));
        }

        if (string.IsNullOrWhiteSpace(message.Body))
        {
            throw new ArgumentException("Body is required.", nameof(message));
        }
    }
}
