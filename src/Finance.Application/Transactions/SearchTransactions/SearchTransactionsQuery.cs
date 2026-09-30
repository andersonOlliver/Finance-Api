using Finance.Application.Abstractions.Messaging;

namespace Finance.Application.Transactions.SearchTransactions;

public sealed record SearchTransactionsQuery(
    DateTime? From,
    DateTime? To,
    Guid? CategoryId,
    Guid? PaymentId,
    Guid? VehicleId) : IQuery<IReadOnlyList<TransactionResponse>>;
