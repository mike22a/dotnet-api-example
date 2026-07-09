using LibraryApi.Domain.Entities;
using LibraryApi.Domain.Interfaces.Repositories;
using LibraryApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Infrastructure.Repositories;

public class BorrowingRepository : IBorrowingRepository
{
    private readonly AppDbContext _context;

    public BorrowingRepository(AppDbContext context) => _context = context;

    public async Task<(List<Borrowing> Borrowings, int Total)> GetAllAsync(string? search, BorrowingStatus? status, int page, int pageSize)
    {
        var query = _context.Borrowings
            .Include(b => b.Book)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(b =>
                b.MemberName.Contains(search) ||
                b.Book.Title.Contains(search));
        }

        if (status.HasValue)
        {
            query = query.Where(b => b.Status == status.Value);
        }

        var total = await query.CountAsync();

        var borrowings = await query
            .OrderByDescending(b => b.BorrowDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (borrowings, total);
    }

    public async Task<Borrowing?> FindByIdAsync(int id) =>
        await _context.Borrowings
            .Include(b => b.Book)
            .FirstOrDefaultAsync(b => b.Id == id);

    public async Task<List<Borrowing>> FindOverdueBorrowingsAsync() =>
        await _context.Borrowings
            .Where(b => b.Status == BorrowingStatus.Borrowed && b.DueDate < DateTime.UtcNow)
            .ToListAsync();

    public async Task<Borrowing> CreateAsync(Borrowing borrowing)
    {
        _context.Borrowings.Add(borrowing);
        await _context.SaveChangesAsync();
        await _context.Entry(borrowing).Reference(b => b.Book).LoadAsync();
        return borrowing;
    }

    public async Task<Borrowing> UpdateAsync(Borrowing borrowing)
    {
        _context.Borrowings.Update(borrowing);
        await _context.SaveChangesAsync();
        await _context.Entry(borrowing).Reference(b => b.Book).LoadAsync();
        return borrowing;
    }
}
