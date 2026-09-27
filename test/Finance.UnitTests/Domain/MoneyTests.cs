using Finance.Domain.Transactions;
using FluentAssertions;

namespace Finance.UnitTests.Domain;

public class MoneyTests
{
    [Fact]
    public void Add_WithSameCurrency_ShouldSumAmounts()
    {
        var first = new Money(100m, Currency.Usd);
        var second = new Money(50m, Currency.Usd);

        var result = first + second;

        result.Amount.Should().Be(150m);
        result.Currency.Should().Be(Currency.Usd);
    }

    [Fact]
    public void Add_WithDifferentCurrencies_ShouldThrow()
    {
        var first = new Money(100m, Currency.Usd);
        var second = new Money(50m, Currency.Eur);

        var act = () => first + second;

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void IsZero_WithZeroAmount_ShouldBeTrue()
    {
        var money = Money.Zero(Currency.Usd);

        money.IsZero().Should().BeTrue();
    }

    [Fact]
    public void IsZero_WithNonZeroAmount_ShouldBeFalse()
    {
        var money = new Money(1m, Currency.Usd);

        money.IsZero().Should().BeFalse();
    }
}
