using Finance.Application.Abstractions.Messaging;

namespace Finance.Application.Categories.SearchCategories;

public sealed record SearchCategoryQuery : IQuery<IReadOnlyList<CategoryResponse>>;
