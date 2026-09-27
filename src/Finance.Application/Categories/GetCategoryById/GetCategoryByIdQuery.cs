using Finance.Application.Abstractions.Messaging;
using Finance.Application.Categories.SearchCategories;

namespace Finance.Application.Categories.GetCategoryById;

public sealed record GetCategoryByIdQuery(Guid Id) : IQuery<CategoryResponse>;
