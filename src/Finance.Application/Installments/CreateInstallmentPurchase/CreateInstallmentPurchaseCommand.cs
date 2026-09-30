using Finance.Application.Abstractions.Messaging;

namespace Finance.Application.Installments.CreateInstallmentPurchase;

public sealed record CreateInstallmentPurchaseCommand(
    string Title,
    decimal TotalAmount,
    string CurrencyCode,
    int InstallmentCount,
    Guid CategoryId,
    Guid CreditCardId,
    string? Description,
    DateTime? FirstInstallmentReleasedOnUtc) : ICommand<CreateInstallmentPurchaseResponse>;
