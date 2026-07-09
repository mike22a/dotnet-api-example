using LibraryApi.Domain.Entities;

namespace LibraryApi.Domain.Interfaces.Repositories;

public interface IBorrowingRepository
{
    Task<(List<Borrowing> Borrowings, int Total)> GetAllAsync(string? search, BorrowingStatus? status, int page, int pageSize);
    Task<Borrowing?> FindByIdAsync(int id);
    Task<List<Borrowing>> FindOverdueBorrowingsAsync();
    Task<Borrowing> CreateAsync(Borrowing borrowing);
    Task<Borrowing> UpdateAsync(Borrowing borrowing);
}
