using Finance.Application.Abstractions.Messaging;

namespace Finance.Application.Categories.DeleteCategory;

public sealed record DeleteCategoryCommand(Guid Id) : ICommand;
