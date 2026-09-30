using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Abstracts;
using Finance.Domain.Installments;

namespace Finance.Application.Installments.DeleteInstallmentPurchase;

internal sealed class DeleteInstallmentPurchaseCommandHandler(
    IInstallmentPurchaseRepository installmentPurchaseRepository,
    IUserContext userContext,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteInstallmentPurchaseCommand>
{
    public async Task<Result> Handle(DeleteInstallmentPurchaseCommand request, CancellationToken cancellationToken)
    {
        var installmentPurchase = await installmentPurchaseRepository.GetByIdAsync(request.Id, cancellationToken);

        if (installmentPurchase is null || installmentPurchase.UserId != userContext.UserId)
        {
            return Result.Failure(InstallmentPurchaseErrors.NotFound);
        }

        installmentPurchaseRepository.Remove(installmentPurchase);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
