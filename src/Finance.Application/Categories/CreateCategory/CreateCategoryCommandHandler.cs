using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Clock;
using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Abstracts;
using Finance.Domain.Categories;
using Finance.Domain.Shared;

namespace Finance.Application.Categories.CreateCategory;

internal sealed class CreateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateCategoryCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = Category.Create(
            Guid.NewGuid(),
            new Name(request.Name),
            request.Type,
            new Color(request.Color),
            new Icon(request.Icon),
            dateTimeProvider.UtcNow,
            userContext.UserId);

        categoryRepository.Add(category);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}
