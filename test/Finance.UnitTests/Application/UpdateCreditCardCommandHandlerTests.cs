using Finance.Application.CreditCards.UpdateCreditCard;
using Finance.Domain.CreditCards;
using Finance.Domain.Shared;
using FluentAssertions;
using NSubstitute;

namespace Finance.UnitTests.Application;

public class UpdateCreditCardCommandHandlerTests
{
    private readonly ApplicationTestHarness _harness = new();

    private static CreditCard CreateExistingCard(Guid userId) =>
        CreditCard.Create(Guid.NewGuid(), new Name("Nubank"), "Mastercard", 10, userId, DateTime.UtcNow);

    [Fact]
    public async Task Handle_WithOwnedCard_ShouldUpdateItAndSucceed()
    {
        var userId = Guid.NewGuid();
        var card = CreateExistingCard(userId);

        _harness.UserContext.UserId.Returns(userId);
        _harness.CreditCardRepository.GetByIdAsync(card.Id, Arg.Any<CancellationToken>()).Returns(card);

        var command = new UpdateCreditCardCommand(card.Id, "Nubank Ultravioleta", "Visa", 15);

        var result = await _harness.Sender.Send(command);

        result.IsSuccess.Should().BeTrue();
        card.Nickname.Value.Should().Be("Nubank Ultravioleta");
        card.DueDay.Should().Be(15);

        await _harness.UnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCardBelongsToAnotherUser_ShouldReturnNotFound()
    {
        var card = CreateExistingCard(Guid.NewGuid());

        _harness.UserContext.UserId.Returns(Guid.NewGuid());
        _harness.CreditCardRepository.GetByIdAsync(card.Id, Arg.Any<CancellationToken>()).Returns(card);

        var command = new UpdateCreditCardCommand(card.Id, "Hackeado", "Visa", 15);

        var result = await _harness.Sender.Send(command);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CreditCardErrors.NotFound);
    }
}
