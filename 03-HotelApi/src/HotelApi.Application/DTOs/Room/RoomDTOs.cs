using HotelApi.Domain.Entities;
namespace HotelApi.Application.DTOs.Room;

public record CreateRoomRequest(string RoomNumber, int Floor, int RoomTypeId);
public record UpdateRoomRequest(string RoomNumber, int Floor, int RoomTypeId, RoomStatus Status);
public record RoomResponse(int Id, string RoomNumber, int Floor, int RoomTypeId, string RoomTypeName, decimal PricePerNight, string Status, DateTime CreatedAt);
public record AvailableRoomRequest(int RoomTypeId, DateTime CheckInDate, DateTime CheckOutDate);
