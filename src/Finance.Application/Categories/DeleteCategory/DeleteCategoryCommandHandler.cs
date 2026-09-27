using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Abstracts;
using Finance.Domain.Categories;

namespace Finance.Application.Categories.DeleteCategory;

internal sealed class DeleteCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUserContext userContext,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteCategoryCommand>
{
    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category is null || category.UserId != userContext.UserId)
        {
            return Result.Failure(CategoryErrors.NotFound);
        }

        categoryRepository.Remove(category);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
