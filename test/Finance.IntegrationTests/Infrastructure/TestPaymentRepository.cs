using Finance.Domain.Payments;
using Finance.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Finance.IntegrationTests.Infrastructure;

internal sealed class TestPaymentRepository(ApplicationDbContext dbContext) : IPaymentRepository
{
    public void Add(Payment payment) => dbContext.Add(payment);

    public void Remove(Payment payment) => dbContext.Remove(payment);

    public Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Set<Payment>().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
}
