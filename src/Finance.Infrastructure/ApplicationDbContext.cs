using Finance.Application.Exceptions;
using Finance.Domain.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace Finance.Infrastructure;

public sealed class ApplicationDbContext(DbContextOptions options) : DbContext(options), IUnitOfWork
{

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public async Task<int> SaveChangesWithoutEventsAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await AddDomainEventAsOutboxMessages();

            var result = await base.SaveChangesAsync(cancellationToken);


            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException("Concurrency exception ocurred", ex);
        }
    }

    private async Task AddDomainEventAsOutboxMessages()
    {
        //var outboxMessages = ChangeTracker.Entries<Entity>()
        //    .Select(e => e.Entity)
        //    .SelectMany(entity =>
        //    {
        //        var domainEvents = entity.GetDomainEvents();

        //        entity.ClearDomainEvents();

        //        return domainEvents;
        //    })
        //    .Select(domainEvent => new OutboxMessage(
        //                        Guid.NewGuid(),
        //                        _dateTimeProvider.UtcNow,
        //                        domainEvent.GetType().Name,
        //                        JsonConvert.SerializeObject(domainEvent, JsonSerializerSettings)))
        //    .ToList();

        //await AddRangeAsync(outboxMessages);

    }
}
