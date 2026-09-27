namespace Finance.Domain.Transactions;

public interface ITransactionRepository
{
    void Add(Transaction transaction);
    void Remove(Transaction transaction);
    Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
