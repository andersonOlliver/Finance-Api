using Finance.Application.Exceptions;
using Finance.Application.Payments.CreatePayment;
using Finance.Domain.Payments;
using FluentAssertions;
using NSubstitute;

namespace Finance.UnitTests.Application;

public class CreatePaymentCommandHandlerTests
{
    private readonly ApplicationTestHarness _harness = new();

    [Fact]
    public async Task Handle_WithValidCommand_ShouldAddPaymentOwnedByCurrentUser()
    {
        var userId = Guid.NewGuid();
        _harness.UserContext.UserId.Returns(userId);

        var command = new CreatePaymentCommand("Cartão Nubank", PaymentType.CashCredit);

        var result = await _harness.Sender.Send(command);

        result.IsSuccess.Should().BeTrue();

        _harness.PaymentRepository.Received(1).Add(Arg.Is<Payment>(p =>
            p.Id == result.Value &&
            p.Name.Value == "Cartão Nubank" &&
            p.Type == PaymentType.CashCredit &&
            p.UserId == userId));

        await _harness.UnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldGenerateAVersion7Id()
    {
        var command = new CreatePaymentCommand("Cartão Nubank", PaymentType.CashCredit);

        var result = await _harness.Sender.Send(command);

        result.Value.ToString()[14].Should().Be('7');
    }

    [Fact]
    public async Task Handle_WithEmptyName_ShouldThrowValidationException()
    {
        var command = new CreatePaymentCommand(string.Empty, PaymentType.CashCredit);

        var act = () => _harness.Sender.Send(command);

        await act.Should().ThrowAsync<ValidationException>();
        _harness.PaymentRepository.DidNotReceive().Add(Arg.Any<Payment>());
    }

    [Fact]
    public async Task Handle_WithInvalidType_ShouldThrowValidationException()
    {
        var command = new CreatePaymentCommand("Cartão Nubank", (PaymentType)999);

        var act = () => _harness.Sender.Send(command);

        await act.Should().ThrowAsync<ValidationException>();
    }
}
