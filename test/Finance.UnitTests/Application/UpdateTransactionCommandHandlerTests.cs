using Finance.Application.Exceptions;
using Finance.Application.Transactions.UpdateTransaction;
using Finance.Domain.Transactions;
using FluentAssertions;
using NSubstitute;

namespace Finance.UnitTests.Application;

public class UpdateTransactionCommandHandlerTests
{
    private readonly ApplicationTestHarness _harness = new();

    private static Transaction CreateExistingTransaction(Guid userId, Guid categoryId) =>
        Transaction.Create(
            Guid.NewGuid(),
            Title.Create("Mercado").Value,
            new Money(150m, Currency.Usd),
            new Description("Compras do mês"),
            userId,
            categoryId,
            null,
            null,
            DateTime.UtcNow,
            DateTime.UtcNow);

    [Fact]
    public async Task Handle_WithOwnedTransaction_ShouldUpdateItAndSucceed()
    {
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var transaction = CreateExistingTransaction(userId, categoryId);

        _harness.UserContext.UserId.Returns(userId);
        _harness.TransactionRepository.GetByIdAsync(transaction.Id, Arg.Any<CancellationToken>()).Returns(transaction);

        var command = new UpdateTransactionCommand(
            transaction.Id, "Mercado atualizado", 200m, "USD", null, categoryId, null, null, DateTime.UtcNow);

        var result = await _harness.Sender.Send(command);

        result.IsSuccess.Should().BeTrue();
        transaction.Title.Value.Should().Be("Mercado atualizado");
        transaction.Value.Amount.Should().Be(200m);

        await _harness.UnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenTransactionDoesNotExist_ShouldReturnNotFound()
    {
        _harness.UserContext.UserId.Returns(Guid.NewGuid());
        _harness.TransactionRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Transaction?)null);

        var command = new UpdateTransactionCommand(
            Guid.NewGuid(), "Mercado", 150m, "USD", null, Guid.NewGuid(), null, null, DateTime.UtcNow);

        var result = await _harness.Sender.Send(command);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TransactionErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenTransactionBelongsToAnotherUser_ShouldReturnNotFound()
    {
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var transaction = CreateExistingTransaction(ownerId, Guid.NewGuid());

        _harness.UserContext.UserId.Returns(otherUserId);
        _harness.TransactionRepository.GetByIdAsync(transaction.Id, Arg.Any<CancellationToken>()).Returns(transaction);

        var command = new UpdateTransactionCommand(
            transaction.Id, "Mercado", 150m, "USD", null, transaction.CategoryId, null, null, DateTime.UtcNow);

        var result = await _harness.Sender.Send(command);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TransactionErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WithInvalidAmount_ShouldThrowValidationException()
    {
        var command = new UpdateTransactionCommand(
            Guid.NewGuid(), "Mercado", -10m, "USD", null, Guid.NewGuid(), null, null, DateTime.UtcNow);

        var act = () => _harness.Sender.Send(command);

        await act.Should().ThrowAsync<ValidationException>();
        await _harness.TransactionRepository.DidNotReceive().GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
}
