namespace Finance.Domain.Abstracts;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<int> SaveChangesWithoutEventsAsync(CancellationToken cancellationToken = default);
}
