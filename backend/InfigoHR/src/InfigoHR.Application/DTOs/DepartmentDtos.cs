namespace InfigoHR.Application.DTOs;

public sealed record DepartmentDto(int Id, string Name);
public sealed record CreateDepartmentDto(string Name);
public sealed record UpdateDepartmentDto(string Name);
