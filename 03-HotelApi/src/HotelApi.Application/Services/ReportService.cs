using HotelApi.Application.DTOs.Report;
using HotelApi.Domain.Entities;
using HotelApi.Domain.Interfaces.Repositories;

namespace HotelApi.Application.Services;

public class ReportService(IRoomRepository roomRepo, IReservationRepository reservationRepo)
{
    public async Task<OccupancyReportResponse> GetOccupancyAsync()
    {
        var (rooms, total) = await roomRepo.GetAllAsync(null, null, 1, int.MaxValue);
        int occupied    = rooms.Count(r => r.Status == RoomStatus.Occupied);
        int available   = rooms.Count(r => r.Status == RoomStatus.Available);
        int maintenance = rooms.Count(r => r.Status == RoomStatus.Maintenance);

        return new OccupancyReportResponse
        {
            TotalRooms = total,
            OccupiedRooms = occupied,
            AvailableRooms = available,
            MaintenanceRooms = maintenance,
            OccupancyRate = total == 0 ? 0 : Math.Round((double)occupied / total * 100, 2)
        };
    }

    public async Task<RevenueReportResponse> GetRevenueAsync(DateTime from, DateTime to)
    {
        var checkedOut = await reservationRepo.GetCheckedOutInRangeAsync(from, to);

        var byType = checkedOut
            .GroupBy(r => r.Room?.RoomType?.Name ?? "Unknown")
            .Select(g => new RevenueByRoomType
            {
                RoomTypeName = g.Key,
                ReservationCount = g.Count(),
                Revenue = g.Sum(r => r.TotalAmount)
            })
            .OrderByDescending(x => x.Revenue)
            .ToList();

        return new RevenueReportResponse
        {
            From = from, To = to,
            TotalReservations = checkedOut.Count,
            TotalRevenue = checkedOut.Sum(r => r.TotalAmount),
            ByRoomType = byType
        };
    }
}
