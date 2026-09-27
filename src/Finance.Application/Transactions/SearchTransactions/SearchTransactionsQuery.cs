using Finance.Application.Abstractions.Messaging;

namespace Finance.Application.Transactions.SearchTransactions;

public sealed record SearchTransactionsQuery(
    DateTime? From,
    DateTime? To,
    Guid? CategoryId,
    Guid? PaymentId) : IQuery<IReadOnlyList<TransactionResponse>>;
