using LibraryApi.Application.Common;
using LibraryApi.Application.DTOs.Book;
using LibraryApi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly BookService _bookService;

    public BooksController(BookService bookService) => _bookService = bookService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<BookResponse>>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] int? categoryId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var (books, total) = await _bookService.GetAllAsync(search, categoryId, page, pageSize);
        
        var response = ApiResponse<List<BookResponse>>.SuccessResult(books, "Books retrieved successfully.");
        // Add total and pagination headers or metadata if desired
        Response.Headers.Append("X-Total-Count", total.ToString());
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<BookResponse>>> GetById(int id)
    {
        var book = await _bookService.GetByIdAsync(id);
        return Ok(ApiResponse<BookResponse>.SuccessResult(book, "Book retrieved successfully."));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<BookResponse>>> Create([FromBody] CreateBookRequest request)
    {
        var book = await _bookService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = book.Id }, ApiResponse<BookResponse>.SuccessResult(book, "Book created successfully."));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<BookResponse>>> Update(int id, [FromBody] UpdateBookRequest request)
    {
        var book = await _bookService.UpdateAsync(id, request);
        return Ok(ApiResponse<BookResponse>.SuccessResult(book, "Book updated successfully."));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
    {
        await _bookService.DeleteAsync(id);
        return Ok(ApiResponse<string>.SuccessResult("Book deleted successfully."));
    }
}
