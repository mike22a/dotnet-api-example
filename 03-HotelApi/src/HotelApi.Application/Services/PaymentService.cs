using HotelApi.Application.DTOs.Payment;
using HotelApi.Domain.Entities;
using HotelApi.Domain.Exceptions;
using HotelApi.Domain.Interfaces.Repositories;

namespace HotelApi.Application.Services;

public class PaymentService(IPaymentRepository paymentRepo, IReservationRepository reservationRepo)
{
    public async Task<PaymentResponse> CreateAsync(CreatePaymentRequest req)
    {
        var reservation = await reservationRepo.FindByIdAsync(req.ReservationId)
            ?? throw new NotFoundException($"Reservation {req.ReservationId} not found.");

        if (reservation.Status == ReservationStatus.Cancelled)
            throw new BusinessRuleException("Cannot process payment for a cancelled reservation.");

        var existing = await paymentRepo.FindByReservationIdAsync(req.ReservationId);
        if (existing != null && existing.Status == PaymentStatus.Completed)
            throw new ConflictException("Payment has already been completed for this reservation.");

        var payment = new Payment
        {
            ReservationId = req.ReservationId,
            Amount = req.Amount,
            Method = req.Method,
            Status = PaymentStatus.Completed,
            ReferenceNumber = req.ReferenceNumber,
            PaidAt = DateTime.UtcNow
        };

        var created = existing == null
            ? await paymentRepo.CreateAsync(payment)
            : await paymentRepo.UpdateAsync(payment);

        return Map(created);
    }

    public async Task<PaymentResponse> GetByReservationAsync(int reservationId)
    {
        var payment = await paymentRepo.FindByReservationIdAsync(reservationId)
            ?? throw new NotFoundException($"No payment found for reservation {reservationId}.");
        return Map(payment);
    }

    private static PaymentResponse Map(Payment p) => new()
    {
        Id = p.Id, ReservationId = p.ReservationId,
        Amount = p.Amount, Method = p.Method.ToString(),
        Status = p.Status.ToString(), ReferenceNumber = p.ReferenceNumber,
        PaidAt = p.PaidAt
    };
}
