using Finance.Application.Payments.DeletePayment;
using Finance.Domain.Payments;
using Finance.Domain.Shared;
using FluentAssertions;
using NSubstitute;

namespace Finance.UnitTests.Application;

public class DeletePaymentCommandHandlerTests
{
    private readonly ApplicationTestHarness _harness = new();

    private static Payment CreateExistingPayment(Guid? userId) =>
        Payment.Create(Guid.NewGuid(), new Name("Cartão Nubank"), PaymentType.CashCredit, userId, DateTime.UtcNow);

    [Fact]
    public async Task Handle_WithOwnedPayment_ShouldRemoveItAndSucceed()
    {
        var userId = Guid.NewGuid();
        var payment = CreateExistingPayment(userId);

        _harness.UserContext.UserId.Returns(userId);
        _harness.PaymentRepository.GetByIdAsync(payment.Id, Arg.Any<CancellationToken>()).Returns(payment);

        var result = await _harness.Sender.Send(new DeletePaymentCommand(payment.Id));

        result.IsSuccess.Should().BeTrue();
        _harness.PaymentRepository.Received(1).Remove(payment);
    }

    [Fact]
    public async Task Handle_WhenPaymentIsADefaultPayment_ShouldReturnNotFoundAndNotRemove()
    {
        var payment = CreateExistingPayment(null);

        _harness.UserContext.UserId.Returns(Guid.NewGuid());
        _harness.PaymentRepository.GetByIdAsync(payment.Id, Arg.Any<CancellationToken>()).Returns(payment);

        var result = await _harness.Sender.Send(new DeletePaymentCommand(payment.Id));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PaymentErrors.NotFound);
        _harness.PaymentRepository.DidNotReceive().Remove(payment);
    }
}
