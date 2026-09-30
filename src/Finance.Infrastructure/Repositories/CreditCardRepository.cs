using Finance.Domain.CreditCards;

namespace Finance.Infrastructure.Repositories;

internal sealed class CreditCardRepository(ApplicationDbContext context) : Repository<CreditCard>(context), ICreditCardRepository
{
}
