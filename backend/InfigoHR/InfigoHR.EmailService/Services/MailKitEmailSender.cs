using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace EmailService;

public sealed class MailKitEmailSender
{
    private readonly SmtpSettings _settings;

    public MailKitEmailSender(SmtpSettings settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));

        if (string.IsNullOrWhiteSpace(_settings.Host))
        {
            throw new ArgumentException("SMTP host is required.", nameof(settings));
        }

        if (_settings.Port is <= 0 or > 65535)
        {
            throw new ArgumentOutOfRangeException(nameof(settings), "SMTP port is invalid.");
        }

        if (string.IsNullOrWhiteSpace(_settings.FromAddress))
        {
            throw new ArgumentException("From address is required.", nameof(settings));
        }
    }

    public async Task SendAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default)
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

        if (string.IsNullOrWhiteSpace(message.TextBody) &&
            string.IsNullOrWhiteSpace(message.HtmlBody))
        {
            throw new ArgumentException("TextBody or HtmlBody is required.", nameof(message));
        }

        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(new MailboxAddress(_settings.FromName ?? string.Empty, _settings.FromAddress));
        mimeMessage.To.Add(MailboxAddress.Parse(message.To));
        mimeMessage.Subject = message.Subject;

        var bodyBuilder = new BodyBuilder
        {
            TextBody = message.TextBody,
            HtmlBody = message.HtmlBody
        };
        mimeMessage.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        var socketOptions = _settings.UseSsl
            ? SecureSocketOptions.SslOnConnect
            : SecureSocketOptions.Auto;

        await client.ConnectAsync(
            _settings.Host,
            _settings.Port,
            socketOptions,
            cancellationToken);

        if (!string.IsNullOrWhiteSpace(_settings.Username))
        {
            await client.AuthenticateAsync(
                _settings.Username,
                _settings.Password ?? string.Empty,
                cancellationToken);
        }

        await client.SendAsync(mimeMessage, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}
