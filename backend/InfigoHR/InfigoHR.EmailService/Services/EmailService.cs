using InfigoHR.EmailService.Configuration;
using InfigoHR.EmailService.Interfaces;
using InfigoHR.EmailService.Models;
using InfigoHR.EmailService.Validation;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace InfigoHR.EmailService.Services;

public sealed class EmailService : IEmailService
{
    private readonly SmtpSettings _settings;

    public EmailService(IOptions<SmtpSettings> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _settings = options.Value;
        EmailServiceValidator.ValidateSettings(_settings);
    }

    public async Task SendAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default)
    {
        EmailServiceValidator.ValidateMessage(message);

        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(new MailboxAddress(
            _settings.FromName ?? string.Empty,
            _settings.FromAddress));
        mimeMessage.To.Add(MailboxAddress.Parse(message.To));
        mimeMessage.Subject = message.Subject;
        mimeMessage.Body = new TextPart(TextFormat.Html)
        {
            Text = message.Body
        };

        using var client = new SmtpClient();
        var socketOptions = _settings.UseSsl
            ? SecureSocketOptions.SslOnConnect
            : SecureSocketOptions.None;

        await client.ConnectAsync(
            _settings.Host,
            _settings.Port,
            socketOptions,
            cancellationToken);

        try
        {
            if (!string.IsNullOrWhiteSpace(_settings.Username))
            {
                await client.AuthenticateAsync(
                    _settings.Username,
                    _settings.Password ?? string.Empty,
                    cancellationToken);
            }

            await client.SendAsync(mimeMessage, cancellationToken);
        }
        finally
        {
            if (client.IsConnected)
            {
                await client.DisconnectAsync(quit: true, cancellationToken: CancellationToken.None);
            }
        }
    }
}
