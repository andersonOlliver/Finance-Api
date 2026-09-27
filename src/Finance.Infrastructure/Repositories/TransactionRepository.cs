using Finance.Domain.Transactions;

namespace Finance.Infrastructure.Repositories;

internal sealed class TransactionRepository(ApplicationDbContext context) : Repository<Transaction>(context), ITransactionRepository
{
}
