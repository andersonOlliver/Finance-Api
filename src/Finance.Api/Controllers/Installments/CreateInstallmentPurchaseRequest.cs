namespace Finance.Api.Controllers.Installments;

public sealed record CreateInstallmentPurchaseRequest(
    string Title,
    decimal TotalAmount,
    string CurrencyCode,
    int InstallmentCount,
    Guid CategoryId,
    Guid CreditCardId,
    string? Description,
    DateTime? FirstInstallmentReleasedOnUtc);
