namespace InfigoHR.Application.DTOs;

public sealed record AuthDto(int EmployeeId, bool HasPassword, DateTime? ResetTokenExpiry);
public sealed record CreateAuthDto(int EmployeeId, string PasswordHash, string? ResetToken, DateTime? ResetTokenExpiry);
public sealed record UpdateAuthDto(string PasswordHash, string? ResetToken, DateTime? ResetTokenExpiry);
