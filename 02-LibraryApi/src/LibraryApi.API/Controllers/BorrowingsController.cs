using LibraryApi.Application.Common;
using LibraryApi.Application.DTOs.Borrowing;
using LibraryApi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BorrowingsController : ControllerBase
{
    private readonly BorrowingService _borrowingService;

    public BorrowingsController(BorrowingService borrowingService) => _borrowingService = borrowingService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<BorrowingResponse>>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var (borrowings, total) = await _borrowingService.GetAllAsync(search, status, page, pageSize);
        
        var response = ApiResponse<List<BorrowingResponse>>.SuccessResult(borrowings, "Borrowings list retrieved successfully.");
        Response.Headers.Append("X-Total-Count", total.ToString());
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<BorrowingResponse>>> GetById(int id)
    {
        var borrowing = await _borrowingService.GetByIdAsync(id);
        return Ok(ApiResponse<BorrowingResponse>.SuccessResult(borrowing, "Borrowing record retrieved successfully."));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<BorrowingResponse>>> BorrowBook([FromBody] CreateBorrowingRequest request)
    {
        var borrowing = await _borrowingService.BorrowBookAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = borrowing.Id }, ApiResponse<BorrowingResponse>.SuccessResult(borrowing, "Book borrowed successfully."));
    }

    [HttpPut("{id:int}/return")]
    public async Task<ActionResult<ApiResponse<BorrowingResponse>>> ReturnBook(int id, [FromBody] ReturnBorrowingRequest request)
    {
        var borrowing = await _borrowingService.ReturnBookAsync(id, request);
        return Ok(ApiResponse<BorrowingResponse>.SuccessResult(borrowing, "Book returned successfully."));
    }

    [HttpPost("overdue/process")]
    public async Task<ActionResult<ApiResponse<int>>> ProcessOverdue()
    {
        var count = await _borrowingService.ProcessOverdueBorrowingsAsync();
        return Ok(ApiResponse<int>.SuccessResult(count, $"Processed overdue books: {count} updated to Overdue."));
    }
}
