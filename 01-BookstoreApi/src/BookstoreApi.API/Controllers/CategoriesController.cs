using BookstoreApi.Application.Common;
using BookstoreApi.Application.DTOs.Category;
using BookstoreApi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreApi.API.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly CategoryService _categoryService;

    public CategoriesController(CategoryService categoryService) => _categoryService = categoryService;

    /// <summary>Get all categories with their book count.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _categoryService.GetAllAsync();
        return Ok(ApiResponse<List<CategoryResponse>>.SuccessResult(result, "Categories retrieved successfully."));
    }

    /// <summary>Get a category by ID.</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _categoryService.GetByIdAsync(id);
        return Ok(ApiResponse<CategoryResponse>.SuccessResult(result));
    }

    /// <summary>Create a new category. Requires authentication.</summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
    {
        var result = await _categoryService.CreateAsync(request);
        return StatusCode(201, ApiResponse<CategoryResponse>.SuccessResult(result, "Category created successfully."));
    }

    /// <summary>Update an existing category. Requires authentication.</summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryRequest request)
    {
        var result = await _categoryService.UpdateAsync(id, request);
        return Ok(ApiResponse<CategoryResponse>.SuccessResult(result, "Category updated successfully."));
    }

    /// <summary>Delete a category. Fails if the category has books. Requires authentication.</summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        await _categoryService.DeleteAsync(id);
        return Ok(ApiResponse<object>.SuccessResult(null!, "Category deleted successfully."));
    }
}
