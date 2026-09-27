using Finance.Application.Abstractions.Messaging;
using Finance.Application.Transactions.SearchTransactions;

namespace Finance.Application.Transactions.GetTransactionById;

public sealed record GetTransactionByIdQuery(Guid Id) : IQuery<TransactionResponse>;
