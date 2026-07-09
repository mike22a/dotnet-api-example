namespace LibraryApi.Application.DTOs.Borrowing;

public class CreateBorrowingRequest
{
    public int BookId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public int DurationDays { get; set; } = 7;
}

public class ReturnBorrowingRequest
{
    public DateTime ReturnDate { get; set; }
}

public class BorrowingResponse
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;
    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal FineAmount { get; set; }
    public DateTime CreatedAt { get; set; }
}
