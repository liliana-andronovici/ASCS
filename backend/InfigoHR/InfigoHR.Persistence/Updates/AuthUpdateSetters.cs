using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace InfigoHR.Persistence.Updates;

public static class AuthUpdateSetters
{
    public static void Apply(
        UpdateSettersBuilder<AuthEntity> setters,
        AuthEntity auth)
    {
        setters
            .SetProperty(current => current.PasswordHash, auth.PasswordHash)
            .SetProperty(current => current.ResetToken, auth.ResetToken)
            .SetProperty(
                current => current.ResetTokenExpiresAtUtc,
                auth.ResetTokenExpiresAtUtc)
            .SetProperty(current => current.ResetCodeHash, auth.ResetCodeHash)
            .SetProperty(
                current => current.ResetCodeExpiresAtUtc,
                auth.ResetCodeExpiresAtUtc)
            .SetProperty(
                current => current.ResetRequestedAtUtc,
                auth.ResetRequestedAtUtc)
            .SetProperty(current => current.ResetAttempts, auth.ResetAttempts)
            .SetProperty(current => current.TokenVersion, auth.TokenVersion)
            .SetProperty(
                current => current.ConcurrencyVersion,
                auth.ConcurrencyVersion)
            .SetProperty(current => current.UpdatedOnUtc, auth.UpdatedOnUtc);
    }
}