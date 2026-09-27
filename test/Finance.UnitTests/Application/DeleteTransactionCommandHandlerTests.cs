using Finance.Application.Transactions.DeleteTransaction;
using Finance.Domain.Transactions;
using FluentAssertions;
using NSubstitute;

namespace Finance.UnitTests.Application;

public class DeleteTransactionCommandHandlerTests
{
    private readonly ApplicationTestHarness _harness = new();

    private static Transaction CreateExistingTransaction(Guid userId) =>
        Transaction.Create(
            Guid.NewGuid(),
            Title.Create("Mercado").Value,
            new Money(150m, Currency.Usd),
            null,
            userId,
            Guid.NewGuid(),
            null,
            DateTime.UtcNow,
            DateTime.UtcNow);

    [Fact]
    public async Task Handle_WithOwnedTransaction_ShouldRemoveItAndSucceed()
    {
        var userId = Guid.NewGuid();
        var transaction = CreateExistingTransaction(userId);

        _harness.UserContext.UserId.Returns(userId);
        _harness.TransactionRepository.GetByIdAsync(transaction.Id, Arg.Any<CancellationToken>()).Returns(transaction);

        var result = await _harness.Sender.Send(new DeleteTransactionCommand(transaction.Id));

        result.IsSuccess.Should().BeTrue();
        _harness.TransactionRepository.Received(1).Remove(transaction);
        await _harness.UnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenTransactionDoesNotExist_ShouldReturnNotFound()
    {
        _harness.UserContext.UserId.Returns(Guid.NewGuid());
        _harness.TransactionRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Transaction?)null);

        var result = await _harness.Sender.Send(new DeleteTransactionCommand(Guid.NewGuid()));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TransactionErrors.NotFound);
        _harness.TransactionRepository.DidNotReceiveWithAnyArgs().Remove(default!);
    }

    [Fact]
    public async Task Handle_WhenTransactionBelongsToAnotherUser_ShouldReturnNotFoundAndNotRemove()
    {
        var ownerId = Guid.NewGuid();
        var transaction = CreateExistingTransaction(ownerId);

        _harness.UserContext.UserId.Returns(Guid.NewGuid());
        _harness.TransactionRepository.GetByIdAsync(transaction.Id, Arg.Any<CancellationToken>()).Returns(transaction);

        var result = await _harness.Sender.Send(new DeleteTransactionCommand(transaction.Id));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TransactionErrors.NotFound);
        _harness.TransactionRepository.DidNotReceive().Remove(transaction);
    }
}
