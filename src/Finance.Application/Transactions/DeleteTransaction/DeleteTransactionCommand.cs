using Finance.Application.Abstractions.Messaging;

namespace Finance.Application.Transactions.DeleteTransaction;

public sealed record DeleteTransactionCommand(Guid Id) : ICommand;
