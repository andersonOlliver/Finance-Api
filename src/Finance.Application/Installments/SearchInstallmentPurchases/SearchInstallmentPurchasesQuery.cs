using Finance.Application.Abstractions.Messaging;

namespace Finance.Application.Installments.SearchInstallmentPurchases;

public sealed record SearchInstallmentPurchasesQuery(
    DateTime? Month,
    Guid? CreditCardId) : IQuery<IReadOnlyList<InstallmentPurchaseStatusResponse>>;
