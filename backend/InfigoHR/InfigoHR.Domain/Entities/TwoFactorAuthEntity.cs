using InfigoHR.Domain.Common;

namespace InfigoHR.Domain.Entities;

public class TwoFactorAuthEntity : BaseEntity
{
    public int EmployeeId { get; set; }
    public virtual EmployeeEntity? Employee { get; set; }

    public string ProtectedSecret { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }

    public Guid SetupId { get; set; }
    public Guid SetupTokenVersion { get; set; }
    public DateTime? SetupExpiresAtUtc { get; set; }

    public long? LastAcceptedTimeStep { get; set; }

    public int FailedAttempts { get; set; }
    public DateTime? LockedUntilUtc { get; set; }

    public string[] RecoveryCodeHashes { get; set; } = [];

    public string? ChallengeTokenHash { get; set; }
    public Guid? ChallengeTokenVersion { get; set; }
    public DateTime? ChallengeExpiresAtUtc { get; set; }

    public bool IsLocked(DateTime now)
    {
        return LockedUntilUtc.HasValue &&
               LockedUntilUtc.Value > now;
    }

    public void RegisterFailure(
        DateTime now,
        int maximumAttempts,
        int lockoutMinutes)
    {
        var previousLockoutExpired =
            LockedUntilUtc.HasValue &&
            LockedUntilUtc.Value <= now;

        if (previousLockoutExpired)
        {
            ClearFailures();
        }

        FailedAttempts++;

        if (FailedAttempts >= maximumAttempts)
        {
            LockedUntilUtc = now.AddMinutes(lockoutMinutes);
        }
    }

    public void ClearFailures()
    {
        FailedAttempts = 0;
        LockedUntilUtc = null;
    }

    public bool TryAcceptTimeStep(long timeStep)
    {
        var alreadyAccepted =
            LastAcceptedTimeStep.HasValue &&
            timeStep <= LastAcceptedTimeStep.Value;

        if (alreadyAccepted)
        {
            return false;
        }

        LastAcceptedTimeStep = timeStep;

        return true;
    }

    public bool TryConsumeRecoveryCode(string hash)
    {
        if (!RecoveryCodeHashes.Contains(hash))
        {
            return false;
        }

        RecoveryCodeHashes = RecoveryCodeHashes
            .Where(current => current != hash)
            .ToArray();

        return true;
    }

    public void ClearChallenge()
    {
        ChallengeTokenHash = null;
        ChallengeTokenVersion = null;
        ChallengeExpiresAtUtc = null;
    }

    public void Disable()
    {
        IsEnabled = false;
        ProtectedSecret = string.Empty;

        SetupId = Guid.Empty;
        SetupTokenVersion = Guid.Empty;
        SetupExpiresAtUtc = null;

        LastAcceptedTimeStep = null;
        RecoveryCodeHashes = [];

        ClearFailures();
        ClearChallenge();
    }
}