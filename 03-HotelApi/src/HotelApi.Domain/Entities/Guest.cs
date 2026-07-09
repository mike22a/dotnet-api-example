namespace HotelApi.Domain.Entities;

public class Guest
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string IdentityNumber { get; set; } = string.Empty; // KTP/Passport
    public string Nationality { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
