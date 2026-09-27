using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Abstracts;
using Finance.Domain.Transactions;

namespace Finance.Application.Transactions.DeleteTransaction;

internal sealed class DeleteTransactionCommandHandler(
    ITransactionRepository transactionRepository,
    IUserContext userContext,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteTransactionCommand>
{
    public async Task<Result> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await transactionRepository.GetByIdAsync(request.Id, cancellationToken);

        if (transaction is null || transaction.UserId != userContext.UserId)
        {
            return Result.Failure(TransactionErrors.NotFound);
        }

        transactionRepository.Remove(transaction);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
