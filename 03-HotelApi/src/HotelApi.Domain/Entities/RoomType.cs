namespace HotelApi.Domain.Entities;

public class RoomType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;       // Standard, Deluxe, Suite
    public string Description { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int MaxOccupancy { get; set; }
    public ICollection<Room> Rooms { get; set; } = new List<Room>();
}
