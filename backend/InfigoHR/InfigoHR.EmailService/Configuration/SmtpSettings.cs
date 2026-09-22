namespace EmailService;

public sealed class SmtpSettings
{
    public required string Host { get; init; }
    public int Port { get; init; } = 25;
    public bool UseSsl { get; init; }
    public string? Username { get; init; }
    public string? Password { get; init; }
    public required string FromAddress { get; init; }
    public string? FromName { get; init; }
}
