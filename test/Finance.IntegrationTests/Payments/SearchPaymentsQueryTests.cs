using Finance.Application.Payments.SearchPayments;
using Finance.Domain.Payments;
using Finance.Domain.Shared;
using Finance.Infrastructure;
using Finance.IntegrationTests.Infrastructure;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Finance.IntegrationTests.Payments;

[Collection("Database")]
public class SearchPaymentsQueryTests(DatabaseFixture fixture)
{
    [Fact]
    public async Task Should_return_seeded_payments()
    {
        var paymentId = Guid.NewGuid();
        using (var scope = fixture.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Set<Payment>().Add(Payment.Create(
                paymentId, new Name("Pix"), PaymentType.Pix, null, DateTime.UtcNow));
            await dbContext.SaveChangesWithoutEventsAsync();
        }

        using var queryScope = fixture.Services.CreateScope();
        var sender = queryScope.ServiceProvider.GetRequiredService<ISender>();

        var result = await sender.Send(new SearchPaymentsQuery());

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Contain(p => p.Id == paymentId && p.Name == "Pix" && p.Type == PaymentType.Pix);
    }
}
