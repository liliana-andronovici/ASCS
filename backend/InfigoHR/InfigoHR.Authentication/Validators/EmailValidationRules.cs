namespace InfigoHR.Authentication.Validators;

public static class EmailValidationRules
{
    public static bool NotContainWhitespace(string? email)
    {
        return email is null || !email.Any(char.IsWhiteSpace);
    }
}