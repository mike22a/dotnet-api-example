namespace HotelApi.Domain.Entities;

public enum ReservationStatus
{
    Pending,    // Created, awaiting confirmation
    Confirmed,  // Confirmed by receptionist
    CheckedIn,  // Guest has arrived
    CheckedOut, // Guest has departed
    Cancelled   // Reservation cancelled
}

public class Reservation
{
    public int Id { get; set; }
    public int GuestId { get; set; }
    public Guest Guest { get; set; } = null!;
    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int Nights => (CheckOutDate.Date - CheckInDate.Date).Days;
    public decimal TotalAmount { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    public string? Notes { get; set; }
    public int? CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Payment? Payment { get; set; }
}
