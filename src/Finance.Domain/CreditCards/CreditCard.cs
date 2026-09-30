using Finance.Domain.Abstracts;
using Finance.Domain.Shared;

namespace Finance.Domain.CreditCards;

public sealed class CreditCard : Entity
{
    private CreditCard(
        Guid id,
        Name nickname,
        string brand,
        int dueDay,
        Guid userId,
        DateTime createdOnUtc)
        : base(id)
    {
        Nickname = nickname;
        Brand = brand;
        DueDay = dueDay;
        UserId = userId;
        CreatedOnUtc = createdOnUtc;
    }

    private CreditCard() { }

    public Name Nickname { get; private set; }
    public string Brand { get; private set; }
    public int DueDay { get; private set; }
    public Guid UserId { get; init; }
    public DateTime CreatedOnUtc { get; init; }
    public DateTime? UpdatedOnUtc { get; private set; }

    public static CreditCard Create(Guid id, Name nickname, string brand, int dueDay, Guid userId, DateTime createdOnUtc)
    {
        return new CreditCard(id, nickname, brand, dueDay, userId, createdOnUtc);
    }

    public void Update(Name nickname, string brand, int dueDay, DateTime updatedOnUtc)
    {
        Nickname = nickname;
        Brand = brand;
        DueDay = dueDay;
        UpdatedOnUtc = updatedOnUtc;
    }

    /// <summary>
    /// The next date this card's bill is due, on or after <paramref name="referenceDate"/>.
    /// Clamps DueDay to the last valid day of a shorter month (e.g. DueDay 31 in February).
    /// </summary>
    public DateTime GetNextDueDate(DateTime referenceDate)
    {
        var dueDateThisMonth = BuildDueDate(referenceDate.Year, referenceDate.Month);

        if (referenceDate.Date <= dueDateThisMonth.Date)
        {
            return dueDateThisMonth;
        }

        var nextMonth = referenceDate.AddMonths(1);
        return BuildDueDate(nextMonth.Year, nextMonth.Month);
    }

    private DateTime BuildDueDate(int year, int month)
    {
        var day = Math.Min(DueDay, DateTime.DaysInMonth(year, month));
        return new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
    }
}
