using InfigoHR.Application.Models.Rooms;

namespace InfigoHR.Application.Interfaces.Services;

public interface IRoomService
{
    Task<List<RoomModel>> GetRoomsAsync(CancellationToken cancellationToken);
    Task<int> CreateRoomAsync(CreateRoomModel model, CancellationToken cancellationToken);
    Task<List<RoomBookingModel>> GetBookingsAsync(DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken);
    Task<int> CreateBookingAsync(CreateRoomBookingModel model, int organizerEmployeeId, CancellationToken cancellationToken);
}
