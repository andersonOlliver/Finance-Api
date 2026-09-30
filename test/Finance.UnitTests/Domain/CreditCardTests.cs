using Finance.Domain.CreditCards;
using Finance.Domain.Shared;
using FluentAssertions;

namespace Finance.UnitTests.Domain;

public class CreditCardTests
{
    private static CreditCard CreateCard(int dueDay) =>
        CreditCard.Create(Guid.NewGuid(), new Name("Nubank"), "Mastercard", dueDay, Guid.NewGuid(), DateTime.UtcNow);

    [Fact]
    public void Create_ShouldSetAllProperties()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var createdOnUtc = DateTime.UtcNow;

        var card = CreditCard.Create(id, new Name("Nubank"), "Mastercard", 10, userId, createdOnUtc);

        card.Id.Should().Be(id);
        card.Nickname.Value.Should().Be("Nubank");
        card.Brand.Should().Be("Mastercard");
        card.DueDay.Should().Be(10);
        card.UserId.Should().Be(userId);
        card.CreatedOnUtc.Should().Be(createdOnUtc);
        card.UpdatedOnUtc.Should().BeNull();
    }

    [Fact]
    public void Update_ShouldReplaceFieldsAndSetUpdatedOnUtc()
    {
        var card = CreateCard(10);
        var updatedOnUtc = DateTime.UtcNow.AddMinutes(5);

        card.Update(new Name("Nubank Ultravioleta"), "Visa", 15, updatedOnUtc);

        card.Nickname.Value.Should().Be("Nubank Ultravioleta");
        card.Brand.Should().Be("Visa");
        card.DueDay.Should().Be(15);
        card.UpdatedOnUtc.Should().Be(updatedOnUtc);
    }

    [Fact]
    public void GetNextDueDate_WhenDueDayHasNotPassedThisMonth_ShouldReturnThisMonth()
    {
        var card = CreateCard(20);
        var referenceDate = new DateTime(2026, 6, 10, 0, 0, 0, DateTimeKind.Utc);

        var nextDueDate = card.GetNextDueDate(referenceDate);

        nextDueDate.Should().Be(new DateTime(2026, 6, 20, 0, 0, 0, DateTimeKind.Utc));
    }

    [Fact]
    public void GetNextDueDate_WhenDueDayAlreadyPassedThisMonth_ShouldReturnNextMonth()
    {
        var card = CreateCard(10);
        var referenceDate = new DateTime(2026, 6, 20, 0, 0, 0, DateTimeKind.Utc);

        var nextDueDate = card.GetNextDueDate(referenceDate);

        nextDueDate.Should().Be(new DateTime(2026, 7, 10, 0, 0, 0, DateTimeKind.Utc));
    }

    [Fact]
    public void GetNextDueDate_WhenReferenceDateIsExactlyTheDueDate_ShouldReturnThisMonth()
    {
        var card = CreateCard(10);
        var referenceDate = new DateTime(2026, 6, 10, 0, 0, 0, DateTimeKind.Utc);

        var nextDueDate = card.GetNextDueDate(referenceDate);

        nextDueDate.Should().Be(new DateTime(2026, 6, 10, 0, 0, 0, DateTimeKind.Utc));
    }

    [Fact]
    public void GetNextDueDate_WhenDueDayDoesNotExistInMonth_ShouldClampToLastDayOfMonth()
    {
        var card = CreateCard(31);
        var referenceDate = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc);

        var nextDueDate = card.GetNextDueDate(referenceDate);

        nextDueDate.Should().Be(new DateTime(2026, 2, 28, 0, 0, 0, DateTimeKind.Utc));
    }
}
