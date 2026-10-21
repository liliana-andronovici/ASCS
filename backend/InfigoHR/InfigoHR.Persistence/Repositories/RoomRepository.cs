using InfigoHR.Application.Exceptions;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Persistence.Repositories;

public sealed class RoomRepository(AppDbContext context) : IRoomRepository
{
    public Task<List<RoomEntity>> GetRoomsAsync(CancellationToken cancellationToken) =>
        context.Rooms.AsNoTracking().Include(room => room.Location)
            .OrderBy(room => room.Location.Name).ThenBy(room => room.Name)
            .ToListAsync(cancellationToken);

    public Task<bool> LocationExistsAsync(int locationId, CancellationToken cancellationToken) =>
        context.Locations.AnyAsync(location => location.Id == locationId, cancellationToken);

    public Task<bool> RoomNameExistsAsync(int locationId, string name, CancellationToken cancellationToken) =>
        context.Rooms.AnyAsync(room => room.LocationId == locationId && room.Name.ToLower() == name.ToLower(), cancellationToken);

    public async Task<int> AddRoomAsync(RoomEntity room, CancellationToken cancellationToken)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        var location = await context.Locations.FromSqlInterpolated($"SELECT * FROM \"Locations\" WHERE \"Id\" = {room.LocationId} FOR UPDATE")
            .SingleOrDefaultAsync(cancellationToken);
        if (location is null)
            throw new NotFoundException("Office was not found.");
        if (await RoomNameExistsAsync(room.LocationId, room.Name, cancellationToken))
            throw new ConflictException("A room with this name already exists in the office.");

        context.Rooms.Add(room);
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return room.Id;
    }

    public async Task<List<RoomBookingEntity>> GetBookingsAsync(DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken)
    {
        var possibleBookings = await context.RoomBookings.AsNoTracking()
            .Include(booking => booking.OrganizerEmployee)
            .Include(booking => booking.Room)
            .Where(booking => booking.StartDateTimeUtc < toUtc && booking.StartDateTimeUtc >= fromUtc.AddDays(-1))
            .OrderBy(booking => booking.StartDateTimeUtc)
            .ToListAsync(cancellationToken);
        return possibleBookings.Where(booking => booking.StartDateTimeUtc.AddMinutes(booking.DurationMinutes) > fromUtc).ToList();
    }

    public Task<bool> EmployeeExistsAsync(int employeeId, CancellationToken cancellationToken) =>
        context.Employees.AnyAsync(employee => employee.Id == employeeId, cancellationToken);

    public async Task<int> AddBookingAsync(RoomBookingEntity booking, CancellationToken cancellationToken)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        var room = await context.Rooms.FromSqlInterpolated($"SELECT * FROM \"Rooms\" WHERE \"Id\" = {booking.RoomId} FOR UPDATE")
            .SingleOrDefaultAsync(cancellationToken);
        if (room is null)
            throw new NotFoundException("Room was not found.");

        var endUtc = booking.StartDateTimeUtc.AddMinutes(booking.DurationMinutes);
        var candidates = await context.RoomBookings.AsNoTracking()
            .Where(existing => existing.RoomId == booking.RoomId && existing.StartDateTimeUtc < endUtc && existing.StartDateTimeUtc >= booking.StartDateTimeUtc.AddDays(-1))
            .ToListAsync(cancellationToken);
        if (candidates.Any(existing => existing.StartDateTimeUtc.AddMinutes(existing.DurationMinutes) > booking.StartDateTimeUtc))
            throw new ConflictException("This room is already booked for that time.");

        context.RoomBookings.Add(booking);
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return booking.Id;
    }
}
