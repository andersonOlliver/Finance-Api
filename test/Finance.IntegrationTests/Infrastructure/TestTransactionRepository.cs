using Finance.Domain.Transactions;
using Finance.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Finance.IntegrationTests.Infrastructure;

internal sealed class TestTransactionRepository(ApplicationDbContext dbContext) : ITransactionRepository
{
    public void Add(Transaction transaction) => dbContext.Add(transaction);

    public void Remove(Transaction transaction) => dbContext.Remove(transaction);

    public Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Set<Transaction>().FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
}
