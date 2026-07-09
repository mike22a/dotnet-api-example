namespace HotelApi.Application.DTOs.RoomType;

public record CreateRoomTypeRequest(string Name, string Description, decimal PricePerNight, int MaxOccupancy);
public record UpdateRoomTypeRequest(string Name, string Description, decimal PricePerNight, int MaxOccupancy);
public record RoomTypeResponse(int Id, string Name, string Description, decimal PricePerNight, int MaxOccupancy, int RoomCount);
