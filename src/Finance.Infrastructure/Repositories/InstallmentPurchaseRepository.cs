using Finance.Domain.Installments;

namespace Finance.Infrastructure.Repositories;

internal sealed class InstallmentPurchaseRepository(ApplicationDbContext context) : Repository<InstallmentPurchase>(context), IInstallmentPurchaseRepository
{
}
