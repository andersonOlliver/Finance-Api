using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Clock;
using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Abstracts;
using Finance.Domain.Payments;
using Finance.Domain.Shared;

namespace Finance.Application.Payments.CreatePayment;

internal sealed class CreatePaymentCommandHandler(
    IPaymentRepository paymentRepository,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<CreatePaymentCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = Payment.Create(
            Guid.NewGuid(),
            new Name(request.Name),
            request.Type,
            userContext.UserId,
            dateTimeProvider.UtcNow);

        paymentRepository.Add(payment);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return payment.Id;
    }
}
