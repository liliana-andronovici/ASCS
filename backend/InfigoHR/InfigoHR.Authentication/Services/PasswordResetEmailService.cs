using System.Net;
using System.Globalization;
using InfigoHR.Authentication.Interfaces;
using InfigoHR.EmailService.Interfaces;
using InfigoHR.EmailService.Models;
using InfigoHR.EmailTemplates;
using Microsoft.Extensions.Logging;

namespace InfigoHR.Authentication.Services;

public class PasswordResetEmailService : IPasswordResetEmailService
{
    private readonly IEmailService _emailService;
    private readonly ILogger<PasswordResetEmailService> _logger;

    public PasswordResetEmailService(
        IEmailService emailService,
        ILogger<PasswordResetEmailService> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public Task SendCodeAsync(
        string recipient,
        string code,
        DateTime expiresAtUtc,
        CancellationToken cancellationToken = default)
    {
        var expiration = expiresAtUtc.ToString(
            "yyyy-MM-dd HH:mm:ss",
            CultureInfo.InvariantCulture);

        var body = PasswordResetTemplates.CodeBody
            .Replace("{{Code}}", code, StringComparison.Ordinal)
            .Replace(
                "{{ExpiresAtUtc}}",
                expiration,
                StringComparison.Ordinal);

        var message = new EmailMessage
        {
            To = recipient,
            Subject = PasswordResetTemplates.CodeSubject,
            Body = WebUtility.HtmlEncode(body).ReplaceLineEndings("<br>")
        };

        return SendAsync(message, cancellationToken);
    }

    public Task SendPasswordChangedAsync(
        string recipient,
        CancellationToken cancellationToken = default)
    {
        var body = PasswordResetTemplates.PasswordChangedBody;

        var message = new EmailMessage
        {
            To = recipient,
            Subject = PasswordResetTemplates.PasswordChangedSubject,
            Body = WebUtility.HtmlEncode(body).ReplaceLineEndings("<br>")
        };

        return SendAsync(message, cancellationToken);
    }

    private async Task SendAsync(
        EmailMessage message,
        CancellationToken cancellationToken)
    {
        try
        {
            await _emailService.SendAsync(
                message,
                cancellationToken);
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Password-reset email delivery failed.");
        }
    }
}