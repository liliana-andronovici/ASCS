namespace InfigoHR.EmailTemplates;

public static class PasswordResetTemplates
{
    public const string CodeSubject = "Reset your InfigoHR password";

    public const string CodeBody = """
        Your password-reset code is: {{Code}}

        It expires at {{ExpiresAtUtc}} UTC.
        If you did not request this, you can ignore this email.
        """;

    public const string PasswordChangedSubject = "Your InfigoHR password was changed";

    public const string PasswordChangedBody = """
        Your InfigoHR password was changed successfully.

        Previous sessions have been invalidated.
        If you did not make this change, contact your administrator.
        """;
}
