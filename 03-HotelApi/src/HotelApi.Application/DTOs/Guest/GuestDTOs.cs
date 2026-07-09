namespace HotelApi.Application.DTOs.Guest;

public record CreateGuestRequest(string FullName, string Email, string Phone, string IdentityNumber, string Nationality);
public record UpdateGuestRequest(string FullName, string Email, string Phone, string IdentityNumber, string Nationality);
public record GuestResponse(int Id, string FullName, string Email, string Phone, string IdentityNumber, string Nationality, DateTime CreatedAt);
