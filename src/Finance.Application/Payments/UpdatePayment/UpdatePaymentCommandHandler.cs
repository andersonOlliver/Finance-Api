using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Clock;
using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Abstracts;
using Finance.Domain.Payments;
using Finance.Domain.Shared;

namespace Finance.Application.Payments.UpdatePayment;

internal sealed class UpdatePaymentCommandHandler(
    IPaymentRepository paymentRepository,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdatePaymentCommand>
{
    public async Task<Result> Handle(UpdatePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await paymentRepository.GetByIdAsync(request.Id, cancellationToken);

        if (payment is null || payment.UserId != userContext.UserId)
        {
            return Result.Failure(PaymentErrors.NotFound);
        }

        payment.Update(new Name(request.Name), request.Type, dateTimeProvider.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
