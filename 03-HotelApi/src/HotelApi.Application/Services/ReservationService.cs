using HotelApi.Application.DTOs.Payment;
using HotelApi.Application.DTOs.Reservation;
using HotelApi.Domain.Entities;
using HotelApi.Domain.Exceptions;
using HotelApi.Domain.Interfaces.Repositories;

namespace HotelApi.Application.Services;

public class ReservationService(
    IReservationRepository reservationRepo,
    IRoomRepository roomRepo,
    IGuestRepository guestRepo,
    IPaymentRepository paymentRepo)
{
    public async Task<(List<ReservationResponse> Items, int Total)> GetAllAsync(string? status, int? guestId, int page, int pageSize)
    {
        ReservationStatus? s = null;
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<ReservationStatus>(status, true, out var ps)) s = ps;
        var (items, total) = await reservationRepo.GetAllAsync(s, guestId, page, pageSize);
        return (items.Select(Map).ToList(), total);
    }

    public async Task<ReservationResponse> GetByIdAsync(int id)
    {
        var r = await reservationRepo.FindByIdAsync(id) ?? throw new NotFoundException($"Reservation {id} not found.");
        return Map(r);
    }

    public async Task<ReservationResponse> CreateAsync(CreateReservationRequest req, int createdByUserId)
    {
        if (req.CheckOutDate <= req.CheckInDate)
            throw new BusinessRuleException("Check-out date must be after check-in date.");

        var guest = await guestRepo.FindByIdAsync(req.GuestId) ?? throw new NotFoundException($"Guest {req.GuestId} not found.");
        var room  = await roomRepo.FindByIdAsync(req.RoomId)   ?? throw new NotFoundException($"Room {req.RoomId} not found.");

        if (room.Status == RoomStatus.Maintenance)
            throw new BusinessRuleException($"Room {room.RoomNumber} is under maintenance.");

        if (await reservationRepo.HasConflictAsync(req.RoomId, req.CheckInDate, req.CheckOutDate))
            throw new ConflictException($"Room {room.RoomNumber} is not available for the selected dates.");

        int nights = (req.CheckOutDate.Date - req.CheckInDate.Date).Days;
        decimal total = nights * room.RoomType.PricePerNight;

        var reservation = new Reservation
        {
            GuestId = req.GuestId, RoomId = req.RoomId,
            CheckInDate = req.CheckInDate.Date, CheckOutDate = req.CheckOutDate.Date,
            TotalAmount = total, Notes = req.Notes,
            CreatedByUserId = createdByUserId, Status = ReservationStatus.Pending
        };

        return Map(await reservationRepo.CreateAsync(reservation));
    }

    // ── State Machine Transitions ─────────────────────────────────────────────

    public async Task<ReservationResponse> ConfirmAsync(int id)
    {
        var r = await reservationRepo.FindByIdAsync(id) ?? throw new NotFoundException($"Reservation {id} not found.");
        if (r.Status != ReservationStatus.Pending)
            throw new BusinessRuleException($"Only Pending reservations can be confirmed. Current status: {r.Status}.");
        r.Status = ReservationStatus.Confirmed;
        return Map(await reservationRepo.UpdateAsync(r));
    }

    public async Task<ReservationResponse> CheckInAsync(int id)
    {
        var r = await reservationRepo.FindByIdAsync(id) ?? throw new NotFoundException($"Reservation {id} not found.");
        if (r.Status != ReservationStatus.Confirmed)
            throw new BusinessRuleException($"Only Confirmed reservations can be checked in. Current status: {r.Status}.");

        r.Status = ReservationStatus.CheckedIn;
        var room = await roomRepo.FindByIdAsync(r.RoomId)!;
        if (room != null) { room.Status = RoomStatus.Occupied; await roomRepo.UpdateAsync(room); }
        return Map(await reservationRepo.UpdateAsync(r));
    }

    public async Task<ReservationResponse> CheckOutAsync(int id)
    {
        var r = await reservationRepo.FindByIdAsync(id) ?? throw new NotFoundException($"Reservation {id} not found.");
        if (r.Status != ReservationStatus.CheckedIn)
            throw new BusinessRuleException($"Only CheckedIn reservations can be checked out. Current status: {r.Status}.");

        r.Status = ReservationStatus.CheckedOut;
        var room = await roomRepo.FindByIdAsync(r.RoomId)!;
        if (room != null) { room.Status = RoomStatus.Available; await roomRepo.UpdateAsync(room); }
        return Map(await reservationRepo.UpdateAsync(r));
    }

    public async Task<ReservationResponse> CancelAsync(int id)
    {
        var r = await reservationRepo.FindByIdAsync(id) ?? throw new NotFoundException($"Reservation {id} not found.");
        if (r.Status is ReservationStatus.CheckedIn or ReservationStatus.CheckedOut)
            throw new BusinessRuleException($"Cannot cancel a reservation with status: {r.Status}.");

        r.Status = ReservationStatus.Cancelled;
        if (r.Payment != null && r.Payment.Status == PaymentStatus.Completed)
        {
            r.Payment.Status = PaymentStatus.Refunded;
            await paymentRepo.UpdateAsync(r.Payment);
        }
        return Map(await reservationRepo.UpdateAsync(r));
    }

    private static ReservationResponse Map(Reservation r) => new()
    {
        Id = r.Id, GuestId = r.GuestId,
        GuestName = r.Guest?.FullName ?? "",
        RoomId = r.RoomId, RoomNumber = r.Room?.RoomNumber ?? "",
        RoomTypeName = r.Room?.RoomType?.Name ?? "",
        CheckInDate = r.CheckInDate, CheckOutDate = r.CheckOutDate,
        Nights = (r.CheckOutDate.Date - r.CheckInDate.Date).Days,
        TotalAmount = r.TotalAmount, Status = r.Status.ToString(),
        Notes = r.Notes, CreatedAt = r.CreatedAt,
        Payment = r.Payment == null ? null : new PaymentInfo
        {
            Id = r.Payment.Id, Amount = r.Payment.Amount,
            Method = r.Payment.Method.ToString(), Status = r.Payment.Status.ToString(),
            ReferenceNumber = r.Payment.ReferenceNumber
        }
    };
}
