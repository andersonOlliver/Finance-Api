using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Categories;

namespace Finance.Application.Categories.CreateCategory;

public sealed record CreateCategoryCommand(
    string Name,
    CategoryType Type,
    string Color,
    string Icon) : ICommand<Guid>;
