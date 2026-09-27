using Finance.Domain.Categories;

namespace Finance.Application.Categories.SearchCategories;

public sealed class CategoryResponse
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public CategoryType Type { get; init; }
    public string? Color { get; init; }
    public string? Icon { get; init; }
}