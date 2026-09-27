using Finance.Domain.Transactions;
using FluentAssertions;

namespace Finance.UnitTests.Domain;

public class CurrencyTests
{
    [Theory]
    [InlineData("USD")]
    [InlineData("EUR")]
    [InlineData("BLR")]
    public void FromCode_WithKnownCode_ShouldReturnMatchingCurrency(string code)
    {
        var currency = Currency.FromCode(code);

        currency.Code.Should().Be(code);
    }

    [Fact]
    public void FromCode_WithUnknownCode_ShouldThrow()
    {
        var act = () => Currency.FromCode("XYZ");

        act.Should().Throw<ApplicationException>();
    }
}
