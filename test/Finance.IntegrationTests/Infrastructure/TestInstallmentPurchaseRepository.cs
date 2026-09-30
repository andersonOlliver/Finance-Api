using Finance.Domain.Installments;
using Finance.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Finance.IntegrationTests.Infrastructure;

internal sealed class TestInstallmentPurchaseRepository(ApplicationDbContext dbContext) : IInstallmentPurchaseRepository
{
    public void Add(InstallmentPurchase installmentPurchase) => dbContext.Add(installmentPurchase);

    public void Remove(InstallmentPurchase installmentPurchase) => dbContext.Remove(installmentPurchase);

    public Task<InstallmentPurchase?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Set<InstallmentPurchase>().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
}
