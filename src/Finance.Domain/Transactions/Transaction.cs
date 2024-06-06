using Finance.Domain.Abstracts;

namespace Finance.Domain.Transactions;

public sealed class Transaction : Entity
{
    private Transaction(
        Guid id,
        Title title,
        Money value,
        Description? description,
        Guid userId,
        Guid categoryId,
        Guid? paymentId,
        DateTime releasedOnUtc,
        DateTime createdOnUtc
        )
        : base(id)
    {
        Title = title;
        Value = value;
        Description = description;
        ReleasedOnUtc = releasedOnUtc;
        CreatedOnUtc = createdOnUtc;
        UserId = userId;
        CategoryId = categoryId;
        PaymentId = paymentId;
    }

    private Transaction() { }

    public Title Title { get; init; }
    public Money Value { get; init; }
    public Description? Description { get; init; }
    public Guid UserId { get; init; }
    public Guid CategoryId { get; init; }
    public Guid? PaymentId { get; init; }
    public DateTime ReleasedOnUtc { get; init; }
    public DateTime CreatedOnUtc { get; init; }
    public DateTime? UpdatedOnUtc { get; init; }

    public static Transaction Create(Guid id,
        Title title,
        Money value,
        Description? description,
        Guid userId,
        Guid categoryId,
        Guid? paymentId,
        DateTime releasedOnUtc,
        DateTime createdOnUtc)
    {
        return new Transaction(id, title, value, description, userId, categoryId, paymentId, releasedOnUtc, createdOnUtc);
    }
}
