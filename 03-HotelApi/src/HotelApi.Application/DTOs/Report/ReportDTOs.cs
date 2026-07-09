namespace HotelApi.Application.DTOs.Report;

public class OccupancyReportResponse
{
    public int TotalRooms { get; set; }
    public int OccupiedRooms { get; set; }
    public int AvailableRooms { get; set; }
    public int MaintenanceRooms { get; set; }
    public double OccupancyRate { get; set; }
}

public class RevenueReportResponse
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public int TotalReservations { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<RevenueByRoomType> ByRoomType { get; set; } = new();
}

public class RevenueByRoomType
{
    public string RoomTypeName { get; set; } = string.Empty;
    public int ReservationCount { get; set; }
    public decimal Revenue { get; set; }
}
