using EmailService;
using EmailServiceComponent = EmailService.EmailService;

namespace InfigoHR.Application.Services;

public sealed class InvitationLink
{
    private readonly EmailServiceComponent _emailService;
    private readonly MailKitEmailSender _emailSender;
    private readonly string _baseUrl;

    public InvitationLink(
        EmailServiceComponent emailService,
        MailKitEmailSender emailSender,
        string baseUrl)
    {
        _emailService = emailService;
        _emailSender = emailSender;
        _baseUrl = baseUrl;
    }

    public async Task<EmailServiceCreated> SendAsync(
        string email,
        string? data = null,
        TimeSpan? lifetime = null,
        CancellationToken cancellationToken = default)
    {
        var created = await _emailService.CreateAsync(
            _baseUrl,
            data,
            lifetime ?? TimeSpan.FromHours(24),
            cancellationToken);

        var message = new EmailMessage
        {
            To = email,
            Subject = "You have been invited to InfigoHR",
            TextBody = $"You have been invited to complete your registration. Open this link: {created.Url}",
            HtmlBody = $"""
                <p>Hello,</p>
                <p>You have been invited to complete your registration.</p>
                <p><a href="{created.Url}">Complete registration</a></p>
                """
        };

        await _emailSender.SendAsync(message, cancellationToken);

        return created;
    }
}
