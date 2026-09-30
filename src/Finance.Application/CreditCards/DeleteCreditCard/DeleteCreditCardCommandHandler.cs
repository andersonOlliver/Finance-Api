using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Abstracts;
using Finance.Domain.CreditCards;

namespace Finance.Application.CreditCards.DeleteCreditCard;

internal sealed class DeleteCreditCardCommandHandler(
    ICreditCardRepository creditCardRepository,
    IUserContext userContext,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteCreditCardCommand>
{
    public async Task<Result> Handle(DeleteCreditCardCommand request, CancellationToken cancellationToken)
    {
        var creditCard = await creditCardRepository.GetByIdAsync(request.Id, cancellationToken);

        if (creditCard is null || creditCard.UserId != userContext.UserId)
        {
            return Result.Failure(CreditCardErrors.NotFound);
        }

        creditCardRepository.Remove(creditCard);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
