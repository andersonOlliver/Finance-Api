using Finance.Domain.CreditCards;
using Finance.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Finance.IntegrationTests.Infrastructure;

internal sealed class TestCreditCardRepository(ApplicationDbContext dbContext) : ICreditCardRepository
{
    public void Add(CreditCard creditCard) => dbContext.Add(creditCard);

    public void Remove(CreditCard creditCard) => dbContext.Remove(creditCard);

    public Task<CreditCard?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Set<CreditCard>().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
}
