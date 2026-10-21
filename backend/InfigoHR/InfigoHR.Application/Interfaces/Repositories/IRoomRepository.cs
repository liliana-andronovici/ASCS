using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Interfaces.Repositories;

public interface IRoomRepository
{
    Task<List<RoomEntity>> GetRoomsAsync(CancellationToken cancellationToken);
    Task<bool> LocationExistsAsync(int locationId, CancellationToken cancellationToken);
    Task<bool> RoomNameExistsAsync(int locationId, string name, CancellationToken cancellationToken);
    Task<int> AddRoomAsync(RoomEntity room, CancellationToken cancellationToken);
    Task<List<RoomBookingEntity>> GetBookingsAsync(DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken);
    Task<bool> EmployeeExistsAsync(int employeeId, CancellationToken cancellationToken);
    Task<int> AddBookingAsync(RoomBookingEntity booking, CancellationToken cancellationToken);
}
