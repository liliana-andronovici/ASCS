namespace InfigoHR.Application.DTOs;

public sealed record AuthDto(
    int Id,
    int EmployeeId,
    bool HasPassword,
    DateTime? ResetTokenExpiresAtUtc);

public sealed record CreateAuthDto(
    int EmployeeId,
    string PasswordHash,
    string? ResetToken,
    DateTime? ResetTokenExpiresAtUtc);

public sealed record UpdateAuthDto(
    string PasswordHash,
    string? ResetToken,
    DateTime? ResetTokenExpiresAtUtc);
