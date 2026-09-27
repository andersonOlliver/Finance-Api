using Finance.Domain.Payments;

namespace Finance.Infrastructure.Repositories;

internal sealed class PaymentRepository(ApplicationDbContext context) : Repository<Payment>(context), IPaymentRepository
{
}
