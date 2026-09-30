using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Clock;
using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Abstracts;
using Finance.Domain.Transactions;

namespace Finance.Application.Transactions.UpdateTransaction;

internal sealed class UpdateTransactionCommandHandler(
    ITransactionRepository transactionRepository,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateTransactionCommand>
{
    public async Task<Result> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await transactionRepository.GetByIdAsync(request.Id, cancellationToken);

        if (transaction is null || transaction.UserId != userContext.UserId)
        {
            return Result.Failure(TransactionErrors.NotFound);
        }

        var titleResult = Title.Create(request.Title);

        if (titleResult.IsFailure)
        {
            return Result.Failure(titleResult.Error);
        }

        var currency = Currency.FromCode(request.CurrencyCode);
        var value = new Money(request.Amount, currency);
        var description = request.Description is null ? null : new Description(request.Description);

        transaction.Update(
            titleResult.Value,
            value,
            description,
            request.CategoryId,
            request.PaymentId,
            request.VehicleId,
            request.ReleasedOnUtc,
            dateTimeProvider.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
