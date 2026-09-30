using Finance.Application.Abstractions.Messaging;

namespace Finance.Application.Installments.GetInstallmentPurchaseById;

public sealed record GetInstallmentPurchaseByIdQuery(Guid Id) : IQuery<InstallmentPurchaseDetailResponse>;
