using Finance.Domain.Abstracts;
using Finance.Domain.Transactions;

namespace Finance.Domain.Installments;

public sealed class InstallmentPurchase : Entity
{
    private InstallmentPurchase(
        Guid id,
        Title title,
        Money totalAmount,
        int installmentCount,
        Guid userId,
        Guid categoryId,
        Guid creditCardId,
        DateTime firstInstallmentReleasedOnUtc,
        DateTime createdOnUtc)
        : base(id)
    {
        Title = title;
        TotalAmount = totalAmount;
        InstallmentCount = installmentCount;
        UserId = userId;
        CategoryId = categoryId;
        CreditCardId = creditCardId;
        FirstInstallmentReleasedOnUtc = firstInstallmentReleasedOnUtc;
        CreatedOnUtc = createdOnUtc;
    }

    private InstallmentPurchase() { }

    public Title Title { get; init; }
    public Money TotalAmount { get; init; }
    public int InstallmentCount { get; init; }
    public Guid UserId { get; init; }
    public Guid CategoryId { get; init; }
    public Guid CreditCardId { get; init; }
    public DateTime FirstInstallmentReleasedOnUtc { get; init; }
    public DateTime CreatedOnUtc { get; init; }

    public static InstallmentPurchase Create(
        Guid id,
        Title title,
        Money totalAmount,
        int installmentCount,
        Guid userId,
        Guid categoryId,
        Guid creditCardId,
        DateTime firstInstallmentReleasedOnUtc,
        DateTime createdOnUtc)
    {
        return new InstallmentPurchase(id, title, totalAmount, installmentCount, userId, categoryId, creditCardId, firstInstallmentReleasedOnUtc, createdOnUtc);
    }
}
