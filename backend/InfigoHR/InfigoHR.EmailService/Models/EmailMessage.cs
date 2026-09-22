namespace EmailService;

public sealed class EmailMessage
{
    public required string To { get; init; }
    public required string Subject { get; init; }
    public string? TextBody { get; init; }
    public string? HtmlBody { get; init; }
}
