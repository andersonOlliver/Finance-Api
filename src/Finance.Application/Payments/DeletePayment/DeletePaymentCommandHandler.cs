using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Abstracts;
using Finance.Domain.Payments;

namespace Finance.Application.Payments.DeletePayment;

internal sealed class DeletePaymentCommandHandler(
    IPaymentRepository paymentRepository,
    IUserContext userContext,
    IUnitOfWork unitOfWork) : ICommandHandler<DeletePaymentCommand>
{
    public async Task<Result> Handle(DeletePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await paymentRepository.GetByIdAsync(request.Id, cancellationToken);

        if (payment is null || payment.UserId != userContext.UserId)
        {
            return Result.Failure(PaymentErrors.NotFound);
        }

        paymentRepository.Remove(payment);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
