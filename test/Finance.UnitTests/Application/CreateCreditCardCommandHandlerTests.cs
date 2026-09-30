using Finance.Application.CreditCards.CreateCreditCard;
using Finance.Application.Exceptions;
using Finance.Domain.CreditCards;
using FluentAssertions;
using NSubstitute;

namespace Finance.UnitTests.Application;

public class CreateCreditCardCommandHandlerTests
{
    private readonly ApplicationTestHarness _harness = new();

    [Fact]
    public async Task Handle_WithValidCommand_ShouldAddCreditCardOwnedByCurrentUser()
    {
        var userId = Guid.NewGuid();
        _harness.UserContext.UserId.Returns(userId);

        var command = new CreateCreditCardCommand("Nubank", "Mastercard", 10);

        var result = await _harness.Sender.Send(command);

        result.IsSuccess.Should().BeTrue();

        _harness.CreditCardRepository.Received(1).Add(Arg.Is<CreditCard>(c =>
            c.Id == result.Value &&
            c.Nickname.Value == "Nubank" &&
            c.DueDay == 10 &&
            c.UserId == userId));

        await _harness.UnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithEmptyNickname_ShouldThrowValidationException()
    {
        var command = new CreateCreditCardCommand(string.Empty, "Mastercard", 10);

        var act = () => _harness.Sender.Send(command);

        await act.Should().ThrowAsync<ValidationException>();
        _harness.CreditCardRepository.DidNotReceive().Add(Arg.Any<CreditCard>());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(32)]
    public async Task Handle_WithInvalidDueDay_ShouldThrowValidationException(int dueDay)
    {
        var command = new CreateCreditCardCommand("Nubank", "Mastercard", dueDay);

        var act = () => _harness.Sender.Send(command);

        await act.Should().ThrowAsync<ValidationException>();
    }
}
