namespace HotelApi.Domain.Entities;

public enum RoomStatus { Available, Occupied, Maintenance }

public class Room
{
    public int Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int Floor { get; set; }
    public int RoomTypeId { get; set; }
    public RoomType RoomType { get; set; } = null!;
    public RoomStatus Status { get; set; } = RoomStatus.Available;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
