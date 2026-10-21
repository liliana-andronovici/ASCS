using InfigoHR.Application.Exceptions;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models.Rooms;
using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Services;

public sealed class RoomService(IRoomRepository repository) : IRoomService
{
    public async Task<List<RoomModel>> GetRoomsAsync(CancellationToken cancellationToken)
    {
        var rooms = await repository.GetRoomsAsync(cancellationToken);
        return rooms.Select(room => new RoomModel
        {
            Id = room.Id,
            Name = room.Name,
            Floor = room.Floor,
            Seats = room.Seats,
            Amenities = room.Amenities,
            LocationId = room.LocationId,
            LocationName = room.Location.Name
        }).ToList();
    }

    public async Task<int> CreateRoomAsync(CreateRoomModel model, CancellationToken cancellationToken)
    {
        var name = model.Name?.Trim() ?? string.Empty;
        if (name.Length is < 1 or > 150 || model.LocationId <= 0 || model.Floor is < -20 or > 200 || model.Seats is < 1 or > 1000 || model.Amenities?.Length > 500)
            throw new ValidationException("Enter a valid room name, office, floor, and seat count.");
        if (!await repository.LocationExistsAsync(model.LocationId, cancellationToken))
            throw new NotFoundException("Office was not found.");
        if (await repository.RoomNameExistsAsync(model.LocationId, name, cancellationToken))
            throw new ConflictException("A room with this name already exists in the office.");

        return await repository.AddRoomAsync(new RoomEntity
        {
            Name = name,
            LocationId = model.LocationId,
            Floor = model.Floor,
            Seats = model.Seats,
            Amenities = model.Amenities?.Trim()
        }, cancellationToken);
    }

    public async Task<List<RoomBookingModel>> GetBookingsAsync(DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken)
    {
        if (fromUtc.Kind != DateTimeKind.Utc || toUtc.Kind != DateTimeKind.Utc || toUtc <= fromUtc || toUtc - fromUtc > TimeSpan.FromDays(31))
            throw new ValidationException("Choose a valid booking range of up to 31 days.");
        var bookings = await repository.GetBookingsAsync(fromUtc, toUtc, cancellationToken);
        return bookings.Select(booking => new RoomBookingModel
        {
            Id = booking.Id,
            RoomId = booking.RoomId,
            RoomName = booking.Room.Name,
            OrganizerEmployeeId = booking.OrganizerEmployeeId,
            OrganizerName = $"{booking.OrganizerEmployee.FirstName} {booking.OrganizerEmployee.LastName}",
            Title = booking.Title,
            StartDateTimeUtc = booking.StartDateTimeUtc,
            DurationMinutes = booking.DurationMinutes
        }).ToList();
    }

    public async Task<int> CreateBookingAsync(CreateRoomBookingModel model, int organizerEmployeeId, CancellationToken cancellationToken)
    {
        var title = model.Title?.Trim() ?? string.Empty;
        if (model.RoomId <= 0 || title.Length is < 1 or > 200 || model.StartDateTimeUtc.Kind != DateTimeKind.Utc || model.StartDateTimeUtc <= DateTime.UtcNow || model.DurationMinutes is not (30 or 60 or 90 or 120))
            throw new ValidationException("Enter a future booking with a title and valid duration.");
        if (!await repository.EmployeeExistsAsync(organizerEmployeeId, cancellationToken))
            throw new NotFoundException("Organizer was not found.");
        return await repository.AddBookingAsync(new RoomBookingEntity
        {
            RoomId = model.RoomId,
            OrganizerEmployeeId = organizerEmployeeId,
            Title = title,
            StartDateTimeUtc = model.StartDateTimeUtc,
            DurationMinutes = model.DurationMinutes
        }, cancellationToken);
    }
}
