using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Clock;
using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Abstracts;
using Finance.Domain.Transactions;

namespace Finance.Application.Transactions.CreateTransaction;

internal sealed class CreateTransactionCommandHandler(
    ITransactionRepository transactionRepository,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateTransactionCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var titleResult = Title.Create(request.Title);

        if (titleResult.IsFailure)
        {
            return Result.Failure<Guid>(titleResult.Error);
        }

        var currency = Currency.FromCode(request.CurrencyCode);
        var value = new Money(request.Amount, currency);
        var description = request.Description is null ? null : new Description(request.Description);

        var transaction = Transaction.Create(
            Guid.CreateVersion7(),
            titleResult.Value,
            value,
            description,
            userContext.UserId,
            request.CategoryId,
            request.PaymentId,
            request.ReleasedOnUtc,
            dateTimeProvider.UtcNow);

        transactionRepository.Add(transaction);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return transaction.Id;
    }
}
