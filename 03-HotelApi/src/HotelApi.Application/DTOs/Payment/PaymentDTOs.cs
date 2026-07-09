using HotelApi.Domain.Entities;
namespace HotelApi.Application.DTOs.Payment;

public class CreatePaymentRequest
{
    public int ReservationId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public string? ReferenceNumber { get; set; }
}

public class PaymentResponse
{
    public int Id { get; set; }
    public int ReservationId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ReferenceNumber { get; set; }
    public DateTime PaidAt { get; set; }
}
