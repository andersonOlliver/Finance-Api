using Finance.Application.Abstractions.Messaging;

namespace Finance.Application.Transactions.CreateTransaction;

public sealed record CreateTransactionCommand(
    string Title,
    decimal Amount,
    string CurrencyCode,
    string? Description,
    Guid CategoryId,
    Guid? PaymentId,
    Guid? VehicleId,
    DateTime ReleasedOnUtc) : ICommand<Guid>;
