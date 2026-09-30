using Finance.Application.Installments.DeleteInstallmentPurchase;
using Finance.Domain.Installments;
using Finance.Domain.Shared;
using Finance.Domain.Transactions;
using FluentAssertions;
using NSubstitute;

namespace Finance.UnitTests.Application;

public class DeleteInstallmentPurchaseCommandHandlerTests
{
    private readonly ApplicationTestHarness _harness = new();

    private static InstallmentPurchase CreateExistingPlan(Guid userId) =>
        InstallmentPurchase.Create(
            Guid.NewGuid(),
            Title.Create("Notebook Dell").Value,
            new Money(1000m, Currency.Usd),
            3,
            userId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            DateTime.UtcNow);

    [Fact]
    public async Task Handle_WithOwnedPlan_ShouldRemoveItAndSucceed()
    {
        var userId = Guid.NewGuid();
        var plan = CreateExistingPlan(userId);

        _harness.UserContext.UserId.Returns(userId);
        _harness.InstallmentPurchaseRepository.GetByIdAsync(plan.Id, Arg.Any<CancellationToken>()).Returns(plan);

        var result = await _harness.Sender.Send(new DeleteInstallmentPurchaseCommand(plan.Id));

        result.IsSuccess.Should().BeTrue();
        _harness.InstallmentPurchaseRepository.Received(1).Remove(plan);
    }

    [Fact]
    public async Task Handle_WhenPlanBelongsToAnotherUser_ShouldReturnNotFoundAndNotRemove()
    {
        var plan = CreateExistingPlan(Guid.NewGuid());

        _harness.UserContext.UserId.Returns(Guid.NewGuid());
        _harness.InstallmentPurchaseRepository.GetByIdAsync(plan.Id, Arg.Any<CancellationToken>()).Returns(plan);

        var result = await _harness.Sender.Send(new DeleteInstallmentPurchaseCommand(plan.Id));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(InstallmentPurchaseErrors.NotFound);
        _harness.InstallmentPurchaseRepository.DidNotReceive().Remove(plan);
    }
}
