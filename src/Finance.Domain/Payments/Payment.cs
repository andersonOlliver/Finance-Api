using Finance.Domain.Abstracts;
using Finance.Domain.Shared;

namespace Finance.Domain.Payments;

public sealed class Payment : Entity
{
    private Payment(Guid id, Name name, PaymentType type, Guid? userId, DateTime createdOnUtc) : base(id)
    {
        Name = name;
        Type = type;
        UserId = userId;
        CreatedOnUtc = createdOnUtc;
    }

    private Payment() { }

    public Name Name { get; private set; }
    public PaymentType Type { get; private set; }
    public Guid? UserId { get; init; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? UpdatedOnUtc { get; private set; }

    public static Payment Create(Guid id, Name name, PaymentType type, Guid? userId, DateTime createdOnUtc)
    {
        return new Payment(id, name, type, userId, createdOnUtc);
    }

    public void Update(Name name, PaymentType type, DateTime updatedOnUtc)
    {
        Name = name;
        Type = type;
        UpdatedOnUtc = updatedOnUtc;
    }
}
