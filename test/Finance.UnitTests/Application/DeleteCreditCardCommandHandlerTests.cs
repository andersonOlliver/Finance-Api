using Finance.Application.CreditCards.DeleteCreditCard;
using Finance.Domain.CreditCards;
using Finance.Domain.Shared;
using FluentAssertions;
using NSubstitute;

namespace Finance.UnitTests.Application;

public class DeleteCreditCardCommandHandlerTests
{
    private readonly ApplicationTestHarness _harness = new();

    private static CreditCard CreateExistingCard(Guid userId) =>
        CreditCard.Create(Guid.NewGuid(), new Name("Nubank"), "Mastercard", 10, userId, DateTime.UtcNow);

    [Fact]
    public async Task Handle_WithOwnedCard_ShouldRemoveItAndSucceed()
    {
        var userId = Guid.NewGuid();
        var card = CreateExistingCard(userId);

        _harness.UserContext.UserId.Returns(userId);
        _harness.CreditCardRepository.GetByIdAsync(card.Id, Arg.Any<CancellationToken>()).Returns(card);

        var result = await _harness.Sender.Send(new DeleteCreditCardCommand(card.Id));

        result.IsSuccess.Should().BeTrue();
        _harness.CreditCardRepository.Received(1).Remove(card);
    }

    [Fact]
    public async Task Handle_WhenCardBelongsToAnotherUser_ShouldReturnNotFoundAndNotRemove()
    {
        var card = CreateExistingCard(Guid.NewGuid());

        _harness.UserContext.UserId.Returns(Guid.NewGuid());
        _harness.CreditCardRepository.GetByIdAsync(card.Id, Arg.Any<CancellationToken>()).Returns(card);

        var result = await _harness.Sender.Send(new DeleteCreditCardCommand(card.Id));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CreditCardErrors.NotFound);
        _harness.CreditCardRepository.DidNotReceive().Remove(card);
    }
}
