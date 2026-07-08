using BookstoreApi.Application.Common;
using BookstoreApi.Application.DTOs.Book;
using BookstoreApi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreApi.API.Controllers;

[ApiController]
[Route("api/books")]
public class BooksController : ControllerBase
{
    private readonly BookService _bookService;

    public BooksController(BookService bookService) => _bookService = bookService;

    /// <summary>
    /// Get all books with optional search and filtering.
    /// Query params: search (title/author/ISBN), categoryId, page (default 1), pageSize (default 10).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] int? categoryId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var (books, total) = await _bookService.GetAllAsync(search, categoryId, page, pageSize);

        return Ok(new
        {
            success = true,
            message = "Books retrieved successfully.",
            data = books,
            pagination = new { page, pageSize, total, totalPages = (int)Math.Ceiling((double)total / pageSize) }
        });
    }

    /// <summary>Get a book by ID.</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _bookService.GetByIdAsync(id);
        return Ok(ApiResponse<BookResponse>.SuccessResult(result));
    }

    /// <summary>Add a new book. Requires authentication. ISBN must be unique.</summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateBookRequest request)
    {
        var result = await _bookService.CreateAsync(request);
        return StatusCode(201, ApiResponse<BookResponse>.SuccessResult(result, "Book created successfully."));
    }

    /// <summary>Update a book. Requires authentication.</summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBookRequest request)
    {
        var result = await _bookService.UpdateAsync(id, request);
        return Ok(ApiResponse<BookResponse>.SuccessResult(result, "Book updated successfully."));
    }

    /// <summary>Delete a book. Requires authentication.</summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        await _bookService.DeleteAsync(id);
        return Ok(ApiResponse<object>.SuccessResult(null!, "Book deleted successfully."));
    }
}
