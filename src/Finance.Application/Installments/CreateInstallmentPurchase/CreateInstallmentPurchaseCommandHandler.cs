using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Clock;
using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Abstracts;
using Finance.Domain.CreditCards;
using Finance.Domain.Installments;
using Finance.Domain.Transactions;

namespace Finance.Application.Installments.CreateInstallmentPurchase;

internal sealed class CreateInstallmentPurchaseCommandHandler(
    IInstallmentPurchaseRepository installmentPurchaseRepository,
    ITransactionRepository transactionRepository,
    ICreditCardRepository creditCardRepository,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateInstallmentPurchaseCommand, CreateInstallmentPurchaseResponse>
{
    public async Task<Result<CreateInstallmentPurchaseResponse>> Handle(CreateInstallmentPurchaseCommand request, CancellationToken cancellationToken)
    {
        var creditCard = await creditCardRepository.GetByIdAsync(request.CreditCardId, cancellationToken);

        if (creditCard is null || creditCard.UserId != userContext.UserId)
        {
            return Result.Failure<CreateInstallmentPurchaseResponse>(InstallmentPurchaseErrors.CreditCardNotFound);
        }

        var now = dateTimeProvider.UtcNow;
        var nextDueDate = creditCard.GetNextDueDate(now);
        var firstInstallmentReleasedOnUtc = request.FirstInstallmentReleasedOnUtc ?? nextDueDate;

        if (firstInstallmentReleasedOnUtc.Date < nextDueDate.Date)
        {
            return Result.Failure<CreateInstallmentPurchaseResponse>(InstallmentPurchaseErrors.FirstInstallmentMustBeOnOrAfterNextDueDate);
        }

        var titleResult = Title.Create(request.Title);

        if (titleResult.IsFailure)
        {
            return Result.Failure<CreateInstallmentPurchaseResponse>(titleResult.Error);
        }

        var currency = Currency.FromCode(request.CurrencyCode);
        var totalAmount = new Money(request.TotalAmount, currency);
        var description = request.Description is null ? null : new Description(request.Description);

        var installmentPurchase = InstallmentPurchase.Create(
            Guid.CreateVersion7(),
            titleResult.Value,
            totalAmount,
            request.InstallmentCount,
            userContext.UserId,
            request.CategoryId,
            creditCard.Id,
            firstInstallmentReleasedOnUtc,
            now);

        installmentPurchaseRepository.Add(installmentPurchase);

        var baseInstallmentAmount = Math.Round(request.TotalAmount / request.InstallmentCount, 2, MidpointRounding.AwayFromZero);
        var lastInstallmentAmount = request.TotalAmount - baseInstallmentAmount * (request.InstallmentCount - 1);

        var generatedInstallments = new List<GeneratedInstallmentResponse>();

        for (var i = 0; i < request.InstallmentCount; i++)
        {
            var installmentNumber = i + 1;
            var isLastInstallment = installmentNumber == request.InstallmentCount;
            var installmentAmount = isLastInstallment ? lastInstallmentAmount : baseInstallmentAmount;

            var installmentTitleResult = Title.Create($"{titleResult.Value.Value} ({installmentNumber}/{request.InstallmentCount})");
            var installmentTitle = installmentTitleResult.IsSuccess ? installmentTitleResult.Value : titleResult.Value;
            var releasedOnUtc = firstInstallmentReleasedOnUtc.AddMonths(i);

            var transaction = Transaction.Create(
                Guid.CreateVersion7(),
                installmentTitle,
                new Money(installmentAmount, currency),
                description,
                userContext.UserId,
                request.CategoryId,
                null,
                null,
                releasedOnUtc,
                now,
                installmentPurchase.Id,
                installmentNumber,
                creditCard.Id);

            transactionRepository.Add(transaction);

            generatedInstallments.Add(new GeneratedInstallmentResponse
            {
                TransactionId = transaction.Id,
                InstallmentNumber = installmentNumber,
                Amount = installmentAmount,
                ReleasedOnUtc = releasedOnUtc
            });
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateInstallmentPurchaseResponse
        {
            Id = installmentPurchase.Id,
            Title = titleResult.Value.Value,
            TotalAmount = request.TotalAmount,
            InstallmentCount = request.InstallmentCount,
            Installments = generatedInstallments
        };
    }
}
