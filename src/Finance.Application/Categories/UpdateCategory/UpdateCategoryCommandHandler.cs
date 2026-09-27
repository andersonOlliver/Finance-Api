using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Clock;
using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Abstracts;
using Finance.Domain.Categories;
using Finance.Domain.Shared;

namespace Finance.Application.Categories.UpdateCategory;

internal sealed class UpdateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateCategoryCommand>
{
    public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category is null || category.UserId != userContext.UserId)
        {
            return Result.Failure(CategoryErrors.NotFound);
        }

        category.Update(
            new Name(request.Name),
            request.Type,
            new Color(request.Color),
            new Icon(request.Icon),
            dateTimeProvider.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
