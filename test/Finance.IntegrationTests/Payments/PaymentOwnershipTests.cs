using Finance.Application.Payments.CreatePayment;
using Finance.Application.Payments.DeletePayment;
using Finance.Application.Payments.SearchPayments;
using Finance.Application.Payments.UpdatePayment;
using Finance.Domain.Payments;
using Finance.Domain.Shared;
using Finance.Domain.Users;
using Finance.Infrastructure;
using Finance.IntegrationTests.Infrastructure;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Finance.IntegrationTests.Payments;

[Collection("Database")]
public class PaymentOwnershipTests(DatabaseFixture fixture)
{
    private async Task<Guid> SeedUserAsync()
    {
        using var scope = fixture.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var user = User.Create(
            new FirstName("Test"),
            new LastName("User"),
            new Email($"user-{Guid.NewGuid():N}@test.local"),
            DateTime.UtcNow);
        user.SetIdentityId(string.Empty);

        dbContext.Set<User>().Add(user);
        await dbContext.SaveChangesWithoutEventsAsync();

        return user.Id;
    }

    private async Task<Guid> SeedDefaultPaymentAsync(string name)
    {
        using var scope = fixture.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var payment = Payment.Create(Guid.NewGuid(), new Name(name), PaymentType.Money, null, DateTime.UtcNow);
        dbContext.Set<Payment>().Add(payment);
        await dbContext.SaveChangesWithoutEventsAsync();

        return payment.Id;
    }

    private void SetCurrentUser(Guid userId)
    {
        fixture.Services.GetRequiredService<TestUserContextAccessor>().UserId = userId;
    }

    [Fact]
    public async Task SearchPayments_ShouldReturnDefaultPaymentsAndOnlyTheCurrentUsersOwnPayments()
    {
        var defaultPaymentId = await SeedDefaultPaymentAsync($"Padrão-{Guid.NewGuid():N}");

        var userAId = await SeedUserAsync();
        var userBId = await SeedUserAsync();

        SetCurrentUser(userAId);
        Guid userAPaymentId;
        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();
            var createResult = await sender.Send(new CreatePaymentCommand("Cartão da Usuária A", PaymentType.CashCredit));
            userAPaymentId = createResult.Value;
        }

        SetCurrentUser(userBId);
        Guid userBPaymentId;
        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();
            var createResult = await sender.Send(new CreatePaymentCommand("Cartão do Usuário B", PaymentType.CashCredit));
            userBPaymentId = createResult.Value;
        }

        SetCurrentUser(userAId);
        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();
            var result = await sender.Send(new SearchPaymentsQuery());

            result.Value.Should().Contain(p => p.Id == defaultPaymentId);
            result.Value.Should().Contain(p => p.Id == userAPaymentId);
            result.Value.Should().NotContain(p => p.Id == userBPaymentId);
        }
    }

    [Fact]
    public async Task Payments_WithSameName_FromDifferentUsers_ShouldBeTreatedAsDistinctEntities()
    {
        var userAId = await SeedUserAsync();
        var userBId = await SeedUserAsync();
        const string sharedName = "Cartão Nubank";

        SetCurrentUser(userAId);
        Guid paymentAId;
        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();
            paymentAId = (await sender.Send(new CreatePaymentCommand(sharedName, PaymentType.CashCredit))).Value;
        }

        SetCurrentUser(userBId);
        Guid paymentBId;
        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();
            paymentBId = (await sender.Send(new CreatePaymentCommand(sharedName, PaymentType.CashCredit))).Value;
        }

        paymentAId.Should().NotBe(paymentBId);

        SetCurrentUser(userAId);
        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();

            var updateResult = await sender.Send(new UpdatePaymentCommand(paymentAId, "Cartão Nubank Platinum", PaymentType.InstallmentCredit));
            updateResult.IsSuccess.Should().BeTrue();

            var updateOthersResult = await sender.Send(new UpdatePaymentCommand(paymentBId, "Hackeado", PaymentType.Money));
            updateOthersResult.IsFailure.Should().BeTrue();
        }
    }

    [Fact]
    public async Task DeletePayment_ForADefaultPayment_ShouldReturnNotFound()
    {
        var defaultPaymentId = await SeedDefaultPaymentAsync($"Padrão-{Guid.NewGuid():N}");
        var userId = await SeedUserAsync();

        SetCurrentUser(userId);
        using var scope = fixture.Services.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        var result = await sender.Send(new DeletePaymentCommand(defaultPaymentId));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PaymentErrors.NotFound);
    }
}
