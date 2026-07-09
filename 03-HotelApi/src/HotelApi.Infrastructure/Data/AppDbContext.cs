using HotelApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelApi.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RoomType> RoomTypes => Set<RoomType>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Guest> Guests => Set<Guest>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<User>().HasIndex(u => u.Email).IsUnique();
        mb.Entity<Room>().HasIndex(r => r.RoomNumber).IsUnique();

        mb.Entity<Room>().HasOne(r => r.RoomType).WithMany(rt => rt.Rooms)
            .HasForeignKey(r => r.RoomTypeId).OnDelete(DeleteBehavior.Restrict);

        mb.Entity<Reservation>().HasOne(r => r.Guest).WithMany(g => g.Reservations)
            .HasForeignKey(r => r.GuestId).OnDelete(DeleteBehavior.Restrict);

        mb.Entity<Reservation>().HasOne(r => r.Room).WithMany()
            .HasForeignKey(r => r.RoomId).OnDelete(DeleteBehavior.Restrict);

        mb.Entity<Reservation>().HasOne(r => r.Payment).WithOne(p => p.Reservation)
            .HasForeignKey<Payment>(p => p.ReservationId).OnDelete(DeleteBehavior.Cascade);

        mb.Entity<Reservation>().Ignore(r => r.Nights);

        // Seed RoomTypes
        mb.Entity<RoomType>().HasData(
            new RoomType { Id = 1, Name = "Standard",  Description = "Comfortable standard room",     PricePerNight = 500000,  MaxOccupancy = 2 },
            new RoomType { Id = 2, Name = "Deluxe",    Description = "Spacious deluxe room with view", PricePerNight = 850000,  MaxOccupancy = 2 },
            new RoomType { Id = 3, Name = "Suite",     Description = "Luxury suite with living area",  PricePerNight = 1500000, MaxOccupancy = 4 }
        );

        // Seed Rooms
        mb.Entity<Room>().HasData(
            new Room { Id = 1, RoomNumber = "101", Floor = 1, RoomTypeId = 1, Status = RoomStatus.Available },
            new Room { Id = 2, RoomNumber = "102", Floor = 1, RoomTypeId = 1, Status = RoomStatus.Available },
            new Room { Id = 3, RoomNumber = "201", Floor = 2, RoomTypeId = 2, Status = RoomStatus.Available },
            new Room { Id = 4, RoomNumber = "202", Floor = 2, RoomTypeId = 2, Status = RoomStatus.Available },
            new Room { Id = 5, RoomNumber = "301", Floor = 3, RoomTypeId = 3, Status = RoomStatus.Available }
        );

        // Seed Admin account – password: password123
        mb.Entity<User>().HasData(
            new User { Id = 1, Name = "System Admin",    Email = "admin@hotel.com",       PasswordHash = "$2a$11$k.Ww2Fq3T9.Z1t64v7F2/e1XoH2v4B9T1w.qZ9E1lC/g1vV1b1b1.", Role = UserRole.Admin,         CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new User { Id = 2, Name = "Front Desk",      Email = "receptionist@hotel.com", PasswordHash = "$2a$11$k.Ww2Fq3T9.Z1t64v7F2/e1XoH2v4B9T1w.qZ9E1lC/g1vV1b1b1.", Role = UserRole.Receptionist, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
