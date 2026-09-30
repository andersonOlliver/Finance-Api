using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Clock;
using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Abstracts;
using Finance.Domain.CreditCards;
using Finance.Domain.Shared;

namespace Finance.Application.CreditCards.CreateCreditCard;

internal sealed class CreateCreditCardCommandHandler(
    ICreditCardRepository creditCardRepository,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateCreditCardCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCreditCardCommand request, CancellationToken cancellationToken)
    {
        var creditCard = CreditCard.Create(
            Guid.CreateVersion7(),
            new Name(request.Nickname),
            request.Brand,
            request.DueDay,
            userContext.UserId,
            dateTimeProvider.UtcNow);

        creditCardRepository.Add(creditCard);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return creditCard.Id;
    }
}
