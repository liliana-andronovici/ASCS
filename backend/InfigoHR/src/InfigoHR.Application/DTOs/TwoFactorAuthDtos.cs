namespace InfigoHR.Application.DTOs;

public sealed record TwoFactorAuthDto(int EmployeeId, bool IsConfigured);
public sealed record CreateTwoFactorAuthDto(int EmployeeId, string SecretKey);
public sealed record UpdateTwoFactorAuthDto(string SecretKey);
