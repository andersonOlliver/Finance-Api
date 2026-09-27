using Finance.Domain.Categories;

namespace Finance.Api.Controllers.Categories;

public sealed record CreateCategoryRequest(string Name, CategoryType Type, string Color, string Icon);
