using Finance.Application.Abstractions.Messaging;

namespace Finance.Application.Installments.DeleteInstallmentPurchase;

public sealed record DeleteInstallmentPurchaseCommand(Guid Id) : ICommand;
