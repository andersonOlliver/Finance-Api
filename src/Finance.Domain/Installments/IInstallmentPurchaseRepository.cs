namespace Finance.Domain.Installments;

public interface IInstallmentPurchaseRepository
{
    void Add(InstallmentPurchase installmentPurchase);
    void Remove(InstallmentPurchase installmentPurchase);
    Task<InstallmentPurchase?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
