using Finance.Application.Exceptions;
using Finance.Application.Installments.CreateInstallmentPurchase;
using Finance.Domain.CreditCards;
using Finance.Domain.Installments;
using Finance.Domain.Shared;
using Finance.Domain.Transactions;
using FluentAssertions;
using NSubstitute;

namespace Finance.UnitTests.Application;

public class CreateInstallmentPurchaseCommandHandlerTests
{
    private readonly ApplicationTestHarness _harness = new();

    private static CreditCard CreateCard(Guid userId, int dueDay) =>
        CreditCard.Create(Guid.NewGuid(), new Name("Nubank"), "Mastercard", dueDay, userId, DateTime.UtcNow);

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateInstallmentPurchaseAndAllTransactions()
    {
        var userId = Guid.NewGuid();
        var card = CreateCard(userId, 20);
        var now = new DateTime(2026, 6, 5, 0, 0, 0, DateTimeKind.Utc);
        var nextDueDate = new DateTime(2026, 6, 20, 0, 0, 0, DateTimeKind.Utc);
        var categoryId = Guid.NewGuid();

        _harness.UserContext.UserId.Returns(userId);
        _harness.DateTimeProvider.UtcNow.Returns(now);
        _harness.CreditCardRepository.GetByIdAsync(card.Id, Arg.Any<CancellationToken>()).Returns(card);

        var command = new CreateInstallmentPurchaseCommand(
            "Notebook Dell", 1000m, "USD", 3, categoryId, card.Id, "Compra parcelada", nextDueDate);

        var result = await _harness.Sender.Send(command);

        result.IsSuccess.Should().BeTrue();
        result.Value.InstallmentCount.Should().Be(3);
        result.Value.Installments.Should().HaveCount(3);

        // 1000 / 3 = 333.33, last installment absorbs the rounding remainder
        result.Value.Installments[0].Amount.Should().Be(333.33m);
        result.Value.Installments[1].Amount.Should().Be(333.33m);
        result.Value.Installments[2].Amount.Should().Be(333.34m);
        (result.Value.Installments[0].Amount + result.Value.Installments[1].Amount + result.Value.Installments[2].Amount)
            .Should().Be(1000m);

        result.Value.Installments[0].ReleasedOnUtc.Should().Be(nextDueDate);
        result.Value.Installments[1].ReleasedOnUtc.Should().Be(nextDueDate.AddMonths(1));
        result.Value.Installments[2].ReleasedOnUtc.Should().Be(nextDueDate.AddMonths(2));

        _harness.InstallmentPurchaseRepository.Received(1).Add(Arg.Is<InstallmentPurchase>(p =>
            p.CreditCardId == card.Id &&
            p.CategoryId == categoryId &&
            p.InstallmentCount == 3));

        _harness.TransactionRepository.Received(3).Add(Arg.Is<Transaction>(t =>
            t.CreditCardId == card.Id &&
            t.CategoryId == categoryId &&
            t.UserId == userId));

        _harness.TransactionRepository.Received(1).Add(Arg.Is<Transaction>(t => t.InstallmentNumber == 1));
        _harness.TransactionRepository.Received(1).Add(Arg.Is<Transaction>(t => t.InstallmentNumber == 2));
        _harness.TransactionRepository.Received(1).Add(Arg.Is<Transaction>(t => t.InstallmentNumber == 3));

        await _harness.UnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithoutExplicitFirstInstallmentDate_ShouldDefaultToTheCardsNextDueDate()
    {
        var userId = Guid.NewGuid();
        var card = CreateCard(userId, 20);
        var now = new DateTime(2026, 6, 5, 0, 0, 0, DateTimeKind.Utc);

        _harness.UserContext.UserId.Returns(userId);
        _harness.DateTimeProvider.UtcNow.Returns(now);
        _harness.CreditCardRepository.GetByIdAsync(card.Id, Arg.Any<CancellationToken>()).Returns(card);

        var command = new CreateInstallmentPurchaseCommand(
            "Notebook Dell", 300m, "USD", 3, Guid.NewGuid(), card.Id, null, null);

        var result = await _harness.Sender.Send(command);

        result.IsSuccess.Should().BeTrue();
        result.Value.Installments[0].ReleasedOnUtc.Should().Be(card.GetNextDueDate(now));
    }

    [Fact]
    public async Task Handle_WhenFirstInstallmentDateIsBeforeTheNextDueDate_ShouldFail()
    {
        var userId = Guid.NewGuid();
        var card = CreateCard(userId, 20);
        var now = new DateTime(2026, 6, 5, 0, 0, 0, DateTimeKind.Utc);

        _harness.UserContext.UserId.Returns(userId);
        _harness.DateTimeProvider.UtcNow.Returns(now);
        _harness.CreditCardRepository.GetByIdAsync(card.Id, Arg.Any<CancellationToken>()).Returns(card);

        var tooEarly = new DateTime(2026, 6, 19, 0, 0, 0, DateTimeKind.Utc);
        var command = new CreateInstallmentPurchaseCommand(
            "Notebook Dell", 300m, "USD", 3, Guid.NewGuid(), card.Id, null, tooEarly);

        var result = await _harness.Sender.Send(command);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(InstallmentPurchaseErrors.FirstInstallmentMustBeOnOrAfterNextDueDate);
        _harness.InstallmentPurchaseRepository.DidNotReceive().Add(Arg.Any<InstallmentPurchase>());
        _harness.TransactionRepository.DidNotReceive().Add(Arg.Any<Transaction>());
    }

    [Fact]
    public async Task Handle_WhenCreditCardDoesNotExist_ShouldReturnNotFound()
    {
        _harness.UserContext.UserId.Returns(Guid.NewGuid());
        _harness.CreditCardRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((CreditCard?)null);

        var command = new CreateInstallmentPurchaseCommand(
            "Notebook Dell", 300m, "USD", 3, Guid.NewGuid(), Guid.NewGuid(), null, null);

        var result = await _harness.Sender.Send(command);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(InstallmentPurchaseErrors.CreditCardNotFound);
    }

    [Fact]
    public async Task Handle_WhenCreditCardBelongsToAnotherUser_ShouldReturnNotFound()
    {
        var card = CreateCard(Guid.NewGuid(), 20);
        _harness.UserContext.UserId.Returns(Guid.NewGuid());
        _harness.CreditCardRepository.GetByIdAsync(card.Id, Arg.Any<CancellationToken>()).Returns(card);

        var command = new CreateInstallmentPurchaseCommand(
            "Notebook Dell", 300m, "USD", 3, Guid.NewGuid(), card.Id, null, null);

        var result = await _harness.Sender.Send(command);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(InstallmentPurchaseErrors.CreditCardNotFound);
    }

    [Fact]
    public async Task Handle_WithInstallmentCountBelowMinimum_ShouldThrowValidationException()
    {
        var command = new CreateInstallmentPurchaseCommand(
            "Notebook Dell", 300m, "USD", 1, Guid.NewGuid(), Guid.NewGuid(), null, null);

        var act = () => _harness.Sender.Send(command);

        await act.Should().ThrowAsync<ValidationException>();
        _harness.CreditCardRepository.DidNotReceiveWithAnyArgs().GetByIdAsync(default, default);
    }
}
