namespace Finance.Domain.CreditCards;

public interface ICreditCardRepository
{
    void Add(CreditCard creditCard);
    void Remove(CreditCard creditCard);
    Task<CreditCard?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
