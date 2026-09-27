namespace Finance.Domain.Payments;

public interface IPaymentRepository
{
    void Add(Payment payment);
    void Remove(Payment payment);
    Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
