using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Categories;

namespace Finance.Application.Categories.UpdateCategory;

public sealed record UpdateCategoryCommand(
    Guid Id,
    string Name,
    CategoryType Type,
    string Color,
    string Icon) : ICommand;
