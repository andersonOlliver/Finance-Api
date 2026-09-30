using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Clock;
using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Abstracts;
using Finance.Domain.CreditCards;
using Finance.Domain.Shared;

namespace Finance.Application.CreditCards.UpdateCreditCard;

internal sealed class UpdateCreditCardCommandHandler(
    ICreditCardRepository creditCardRepository,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateCreditCardCommand>
{
    public async Task<Result> Handle(UpdateCreditCardCommand request, CancellationToken cancellationToken)
    {
        var creditCard = await creditCardRepository.GetByIdAsync(request.Id, cancellationToken);

        if (creditCard is null || creditCard.UserId != userContext.UserId)
        {
            return Result.Failure(CreditCardErrors.NotFound);
        }

        creditCard.Update(new Name(request.Nickname), request.Brand, request.DueDay, dateTimeProvider.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
