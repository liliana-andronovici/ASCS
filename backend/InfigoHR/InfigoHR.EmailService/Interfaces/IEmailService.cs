using InfigoHR.EmailService.Models;

namespace InfigoHR.EmailService.Interfaces;

public interface IEmailService
{
    Task SendAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default);
}
