using Finance.Application.Abstractions.Messaging;

namespace Finance.Application.Transactions.UpdateTransaction;

public sealed record UpdateTransactionCommand(
    Guid Id,
    string Title,
    decimal Amount,
    string CurrencyCode,
    string? Description,
    Guid CategoryId,
    Guid? PaymentId,
    Guid? VehicleId,
    DateTime ReleasedOnUtc) : ICommand;
