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
        Guid? vehicleId,
        DateTime releasedOnUtc,
        DateTime createdOnUtc,
        Guid? installmentPurchaseId,
        int? installmentNumber,
        Guid? creditCardId
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
        VehicleId = vehicleId;
        InstallmentPurchaseId = installmentPurchaseId;
        InstallmentNumber = installmentNumber;
        CreditCardId = creditCardId;
    }

    private Transaction() { }

    public Title Title { get; private set; }
    public Money Value { get; private set; }
    public Description? Description { get; private set; }
    public Guid UserId { get; init; }
    public Guid CategoryId { get; private set; }
    public Guid? PaymentId { get; private set; }
    public Guid? VehicleId { get; private set; }
    public Guid? InstallmentPurchaseId { get; init; }
    public int? InstallmentNumber { get; init; }
    public Guid? CreditCardId { get; init; }
    public DateTime ReleasedOnUtc { get; private set; }
    public DateTime CreatedOnUtc { get; init; }
    public DateTime? UpdatedOnUtc { get; private set; }

    public static Transaction Create(Guid id,
        Title title,
        Money value,
        Description? description,
        Guid userId,
        Guid categoryId,
        Guid? paymentId,
        Guid? vehicleId,
        DateTime releasedOnUtc,
        DateTime createdOnUtc,
        Guid? installmentPurchaseId = null,
        int? installmentNumber = null,
        Guid? creditCardId = null)
    {
        return new Transaction(id, title, value, description, userId, categoryId, paymentId, vehicleId, releasedOnUtc, createdOnUtc, installmentPurchaseId, installmentNumber, creditCardId);
    }

    public void Update(
        Title title,
        Money value,
        Description? description,
        Guid categoryId,
        Guid? paymentId,
        Guid? vehicleId,
        DateTime releasedOnUtc,
        DateTime updatedOnUtc)
    {
        Title = title;
        Value = value;
        Description = description;
        CategoryId = categoryId;
        PaymentId = paymentId;
        VehicleId = vehicleId;
        ReleasedOnUtc = releasedOnUtc;
        UpdatedOnUtc = updatedOnUtc;
    }
}
