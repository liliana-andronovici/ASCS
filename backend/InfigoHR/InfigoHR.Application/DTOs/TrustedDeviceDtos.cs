namespace InfigoHR.Application.DTOs;

public sealed record TrustedDeviceDto(int Id, string DeviceToken, int EmployeeId, DateTime TrustedUntilUtc);
public sealed record CreateTrustedDeviceDto(string DeviceToken, int EmployeeId, DateTime TrustedUntilUtc);
public sealed record UpdateTrustedDeviceDto(string DeviceToken, int EmployeeId, DateTime TrustedUntilUtc);
