namespace InfigoHR.Application.Models.Rooms;

public sealed class RoomModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Floor { get; set; }
    public int Seats { get; set; }
    public string? Amenities { get; set; }
    public int LocationId { get; set; }
    public string LocationName { get; set; } = string.Empty;
}

public sealed class CreateRoomModel
{
    public string Name { get; set; } = string.Empty;
    public int Floor { get; set; }
    public int Seats { get; set; }
    public string? Amenities { get; set; }
    public int LocationId { get; set; }
}

public sealed class RoomBookingModel
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public int OrganizerEmployeeId { get; set; }
    public string OrganizerName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime StartDateTimeUtc { get; set; }
    public int DurationMinutes { get; set; }
}

public sealed class CreateRoomBookingModel
{
    public int RoomId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime StartDateTimeUtc { get; set; }
    public int DurationMinutes { get; set; }
}
