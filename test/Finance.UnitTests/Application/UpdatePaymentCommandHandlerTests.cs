using Finance.Application.Payments.UpdatePayment;
using Finance.Domain.Payments;
using Finance.Domain.Shared;
using FluentAssertions;
using NSubstitute;

namespace Finance.UnitTests.Application;

public class UpdatePaymentCommandHandlerTests
{
    private readonly ApplicationTestHarness _harness = new();

    private static Payment CreateExistingPayment(Guid? userId) =>
        Payment.Create(Guid.NewGuid(), new Name("Cartão Nubank"), PaymentType.CashCredit, userId, DateTime.UtcNow);

    [Fact]
    public async Task Handle_WithOwnedPayment_ShouldUpdateItAndSucceed()
    {
        var userId = Guid.NewGuid();
        var payment = CreateExistingPayment(userId);

        _harness.UserContext.UserId.Returns(userId);
        _harness.PaymentRepository.GetByIdAsync(payment.Id, Arg.Any<CancellationToken>()).Returns(payment);

        var command = new UpdatePaymentCommand(payment.Id, "Cartão Nubank Platinum", PaymentType.InstallmentCredit);

        var result = await _harness.Sender.Send(command);

        result.IsSuccess.Should().BeTrue();
        payment.Name.Value.Should().Be("Cartão Nubank Platinum");
        payment.Type.Should().Be(PaymentType.InstallmentCredit);

        await _harness.UnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenPaymentIsADefaultPayment_ShouldReturnNotFound()
    {
        var payment = CreateExistingPayment(null);

        _harness.UserContext.UserId.Returns(Guid.NewGuid());
        _harness.PaymentRepository.GetByIdAsync(payment.Id, Arg.Any<CancellationToken>()).Returns(payment);

        var command = new UpdatePaymentCommand(payment.Id, "Hackeado", PaymentType.Money);

        var result = await _harness.Sender.Send(command);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PaymentErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenPaymentBelongsToAnotherUser_ShouldReturnNotFound()
    {
        var payment = CreateExistingPayment(Guid.NewGuid());

        _harness.UserContext.UserId.Returns(Guid.NewGuid());
        _harness.PaymentRepository.GetByIdAsync(payment.Id, Arg.Any<CancellationToken>()).Returns(payment);

        var command = new UpdatePaymentCommand(payment.Id, "Hackeado", PaymentType.Money);

        var result = await _harness.Sender.Send(command);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PaymentErrors.NotFound);
    }
}
