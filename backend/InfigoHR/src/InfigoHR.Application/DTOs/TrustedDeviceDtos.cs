namespace InfigoHR.Application.DTOs;

public sealed record TrustedDeviceDto(string DeviceToken, int EmployeeId, DateTime TrustedUntil);
public sealed record CreateTrustedDeviceDto(string DeviceToken, int EmployeeId, DateTime TrustedUntil);
public sealed record UpdateTrustedDeviceDto(int EmployeeId, DateTime TrustedUntil);
