using Finance.Domain.Transactions;
using FluentAssertions;

namespace Finance.UnitTests.Domain;

public class TitleTests
{
    [Fact]
    public void Create_WithValidValue_ShouldSucceed()
    {
        var result = Title.Create("Mercado");

        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("Mercado");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithNullOrWhiteSpaceValue_ShouldFailWithNullError(string? value)
    {
        var result = Title.Create(value!);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Title.NullError);
    }

    [Fact]
    public void Create_WithValueLongerThanMaxLength_ShouldFailWithMaxLengthError()
    {
        var value = new string('a', Title.MaxLenght + 1);

        var result = Title.Create(value);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Title.MaxLenghtError);
    }
}
