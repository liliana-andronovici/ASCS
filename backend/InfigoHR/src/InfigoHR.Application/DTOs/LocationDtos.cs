namespace InfigoHR.Application.DTOs;

public sealed record LocationDto(int Id, string Name);
public sealed record CreateLocationDto(string Name);
public sealed record UpdateLocationDto(string Name);
