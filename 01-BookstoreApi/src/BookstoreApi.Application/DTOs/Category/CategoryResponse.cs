namespace BookstoreApi.Application.DTOs.Category;

public class CategoryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int BookCount { get; set; }
}
