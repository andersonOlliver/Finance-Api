using Finance.Domain.Transactions;
using FluentAssertions;

namespace Finance.UnitTests.Domain;

public class TransactionTests
{
    [Fact]
    public void Create_ShouldSetAllProperties()
    {
        var id = Guid.NewGuid();
        var title = Title.Create("Mercado").Value;
        var value = new Money(150m, Currency.Usd);
        var description = new Description("Compras do mês");
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var paymentId = Guid.NewGuid();
        var releasedOnUtc = new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc);
        var createdOnUtc = new DateTime(2026, 1, 10, 12, 0, 0, DateTimeKind.Utc);

        var transaction = Transaction.Create(id, title, value, description, userId, categoryId, paymentId, releasedOnUtc, createdOnUtc);

        transaction.Id.Should().Be(id);
        transaction.Title.Should().Be(title);
        transaction.Value.Should().Be(value);
        transaction.Description.Should().Be(description);
        transaction.UserId.Should().Be(userId);
        transaction.CategoryId.Should().Be(categoryId);
        transaction.PaymentId.Should().Be(paymentId);
        transaction.ReleasedOnUtc.Should().Be(releasedOnUtc);
        transaction.CreatedOnUtc.Should().Be(createdOnUtc);
        transaction.UpdatedOnUtc.Should().BeNull();
    }

    [Fact]
    public void Update_ShouldReplaceMutableFieldsAndSetUpdatedOnUtc()
    {
        var transaction = Transaction.Create(
            Guid.NewGuid(),
            Title.Create("Mercado").Value,
            new Money(150m, Currency.Usd),
            new Description("Compras do mês"),
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            DateTime.UtcNow,
            DateTime.UtcNow);

        var newTitle = Title.Create("Mercado atualizado").Value;
        var newValue = new Money(200m, Currency.Usd);
        var newCategoryId = Guid.NewGuid();
        var newPaymentId = Guid.NewGuid();
        var newReleasedOnUtc = DateTime.UtcNow.AddDays(1);
        var updatedOnUtc = DateTime.UtcNow.AddMinutes(5);

        transaction.Update(newTitle, newValue, null, newCategoryId, newPaymentId, newReleasedOnUtc, updatedOnUtc);

        transaction.Title.Should().Be(newTitle);
        transaction.Value.Should().Be(newValue);
        transaction.Description.Should().BeNull();
        transaction.CategoryId.Should().Be(newCategoryId);
        transaction.PaymentId.Should().Be(newPaymentId);
        transaction.ReleasedOnUtc.Should().Be(newReleasedOnUtc);
        transaction.UpdatedOnUtc.Should().Be(updatedOnUtc);
    }
}
