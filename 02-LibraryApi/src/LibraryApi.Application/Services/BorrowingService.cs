using LibraryApi.Application.DTOs.Borrowing;
using LibraryApi.Domain.Entities;
using LibraryApi.Domain.Exceptions;
using LibraryApi.Domain.Interfaces.Repositories;

namespace LibraryApi.Application.Services;

public class BorrowingService
{
    private readonly IBorrowingRepository _borrowingRepo;
    private readonly IBookRepository _bookRepo;

    public BorrowingService(IBorrowingRepository borrowingRepo, IBookRepository bookRepo)
    {
        _borrowingRepo = borrowingRepo;
        _bookRepo = bookRepo;
    }

    public async Task<(List<BorrowingResponse> Borrowings, int Total)> GetAllAsync(string? search, string? status, int page, int pageSize)
    {
        BorrowingStatus? enumStatus = null;
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (Enum.TryParse<BorrowingStatus>(status, true, out var parsedStatus))
                enumStatus = parsedStatus;
        }

        var (borrowings, total) = await _borrowingRepo.GetAllAsync(search, enumStatus, page, pageSize);
        return (borrowings.Select(MapToResponse).ToList(), total);
    }

    public async Task<BorrowingResponse> GetByIdAsync(int id)
    {
        var borrowing = await _borrowingRepo.FindByIdAsync(id)
            ?? throw new NotFoundException($"Borrowing record with ID {id} not found.");

        return MapToResponse(borrowing);
    }

    public async Task<BorrowingResponse> BorrowBookAsync(CreateBorrowingRequest request)
    {
        var book = await _bookRepo.FindByIdAsync(request.BookId)
            ?? throw new NotFoundException($"Book with ID {request.BookId} not found.");

        if (book.AvailableCopies <= 0)
            throw new BusinessRuleException($"No copies of '{book.Title}' are currently available for borrowing.");

        var borrowing = new Borrowing
        {
            BookId = request.BookId,
            MemberName = request.MemberName,
            BorrowDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(request.DurationDays),
            Status = BorrowingStatus.Borrowed
        };

        // Decrement copies
        book.AvailableCopies -= 1;
        await _bookRepo.UpdateAsync(book);

        var created = await _borrowingRepo.CreateAsync(borrowing);
        return MapToResponse(created);
    }

    public async Task<BorrowingResponse> ReturnBookAsync(int id, ReturnBorrowingRequest request)
    {
        var borrowing = await _borrowingRepo.FindByIdAsync(id)
            ?? throw new NotFoundException($"Borrowing record with ID {id} not found.");

        if (borrowing.Status == BorrowingStatus.Returned)
            throw new BusinessRuleException("This book has already been returned.");

        var book = await _bookRepo.FindByIdAsync(borrowing.BookId)
            ?? throw new NotFoundException($"Book with ID {borrowing.BookId} not found.");

        borrowing.ReturnDate = request.ReturnDate.ToUniversalTime();
        borrowing.Status = BorrowingStatus.Returned;

        // Calculate fine (5000 IDR per day late)
        if (borrowing.ReturnDate.Value.Date > borrowing.DueDate.Date)
        {
            var delay = (borrowing.ReturnDate.Value.Date - borrowing.DueDate.Date).Days;
            if (delay > 0)
            {
                borrowing.FineAmount = delay * 5000m;
            }
        }

        // Increment copies
        book.AvailableCopies = Math.Min(book.TotalCopies, book.AvailableCopies + 1);
        await _bookRepo.UpdateAsync(book);

        var updated = await _borrowingRepo.UpdateAsync(borrowing);
        return MapToResponse(updated);
    }

    public async Task<int> ProcessOverdueBorrowingsAsync()
    {
        var overdue = await _borrowingRepo.FindOverdueBorrowingsAsync();
        int count = 0;

        foreach (var loan in overdue)
        {
            loan.Status = BorrowingStatus.Overdue;
            await _borrowingRepo.UpdateAsync(loan);
            count++;
        }

        return count;
    }

    private static BorrowingResponse MapToResponse(Borrowing b) => new()
    {
        Id = b.Id,
        BookId = b.BookId,
        BookTitle = b.Book?.Title ?? string.Empty,
        MemberName = b.MemberName,
        BorrowDate = b.BorrowDate,
        DueDate = b.DueDate,
        ReturnDate = b.ReturnDate,
        Status = b.Status.ToString(),
        FineAmount = b.FineAmount,
        CreatedAt = b.CreatedAt
    };
}
