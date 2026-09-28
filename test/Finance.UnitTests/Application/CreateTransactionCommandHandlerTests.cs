using Finance.Application.Exceptions;
using Finance.Application.Transactions.CreateTransaction;
using Finance.Domain.Transactions;
using FluentAssertions;
using NSubstitute;

namespace Finance.UnitTests.Application;

public class CreateTransactionCommandHandlerTests
{
    private readonly ApplicationTestHarness _harness = new();

    [Fact]
    public async Task Handle_WithValidCommand_ShouldAddTransactionAndReturnItsId()
    {
        var userId = Guid.NewGuid();
        var now = new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc);
        _harness.UserContext.UserId.Returns(userId);
        _harness.DateTimeProvider.UtcNow.Returns(now);

        var command = new CreateTransactionCommand(
            "Mercado", 150m, "USD", "Compras do mês", Guid.NewGuid(), null, now);

        var result = await _harness.Sender.Send(command);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        _harness.TransactionRepository.Received(1).Add(Arg.Is<Transaction>(t =>
            t.Id == result.Value &&
            t.Title.Value == "Mercado" &&
            t.Value.Amount == 150m &&
            t.UserId == userId &&
            t.CategoryId == command.CategoryId));

        await _harness.UnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldGenerateAVersion7Id()
    {
        var command = new CreateTransactionCommand(
            "Mercado", 150m, "USD", null, Guid.NewGuid(), null, DateTime.UtcNow);

        var result = await _harness.Sender.Send(command);

        result.Value.ToString()[14].Should().Be('7');
    }

    [Fact]
    public async Task Handle_WithEmptyTitle_ShouldThrowValidationException()
    {
        var command = new CreateTransactionCommand(
            string.Empty, 150m, "USD", null, Guid.NewGuid(), null, DateTime.UtcNow);

        var act = () => _harness.Sender.Send(command);

        await act.Should().ThrowAsync<ValidationException>();
        _harness.TransactionRepository.DidNotReceive().Add(Arg.Any<Transaction>());
    }

    [Fact]
    public async Task Handle_WithNonPositiveAmount_ShouldThrowValidationException()
    {
        var command = new CreateTransactionCommand(
            "Mercado", 0m, "USD", null, Guid.NewGuid(), null, DateTime.UtcNow);

        var act = () => _harness.Sender.Send(command);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_WithInvalidCurrencyCode_ShouldThrowValidationException()
    {
        var command = new CreateTransactionCommand(
            "Mercado", 150m, "XYZ", null, Guid.NewGuid(), null, DateTime.UtcNow);

        var act = () => _harness.Sender.Send(command);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_WithEmptyCategoryId_ShouldThrowValidationException()
    {
        var command = new CreateTransactionCommand(
            "Mercado", 150m, "USD", null, Guid.Empty, null, DateTime.UtcNow);

        var act = () => _harness.Sender.Send(command);

        await act.Should().ThrowAsync<ValidationException>();
    }
}
