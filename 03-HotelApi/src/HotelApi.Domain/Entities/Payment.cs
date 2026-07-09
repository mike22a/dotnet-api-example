namespace HotelApi.Domain.Entities;

public enum PaymentMethod { Cash, CreditCard, BankTransfer, DebitCard }
public enum PaymentStatus { Pending, Completed, Refunded }

public class Payment
{
    public int Id { get; set; }
    public int ReservationId { get; set; }
    public Reservation Reservation { get; set; } = null!;
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? ReferenceNumber { get; set; }
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
