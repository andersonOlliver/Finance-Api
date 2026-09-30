using Finance.Application.CreditCards.CreateCreditCard;
using Finance.Application.CreditCards.DeleteCreditCard;
using Finance.Application.CreditCards.SearchCreditCards;
using Finance.Application.CreditCards.UpdateCreditCard;
using Finance.Domain.CreditCards;
using Finance.Domain.Users;
using Finance.Infrastructure;
using Finance.IntegrationTests.Infrastructure;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Finance.IntegrationTests.CreditCards;

[Collection("Database")]
public class CreditCardOwnershipTests(DatabaseFixture fixture)
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

    private void SetCurrentUser(Guid userId)
    {
        fixture.Services.GetRequiredService<TestUserContextAccessor>().UserId = userId;
    }

    [Fact]
    public async Task SearchCreditCards_ShouldReturnOnlyTheCurrentUsersCards()
    {
        var userAId = await SeedUserAsync();
        var userBId = await SeedUserAsync();

        SetCurrentUser(userAId);
        Guid cardAId;
        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();
            cardAId = (await sender.Send(new CreateCreditCardCommand("Nubank da A", "Mastercard", 10))).Value;
        }

        SetCurrentUser(userBId);
        Guid cardBId;
        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();
            cardBId = (await sender.Send(new CreateCreditCardCommand("Itaú do B", "Visa", 5))).Value;
        }

        SetCurrentUser(userAId);
        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();
            var result = await sender.Send(new SearchCreditCardsQuery());

            result.Value.Should().Contain(c => c.Id == cardAId);
            result.Value.Should().NotContain(c => c.Id == cardBId);
        }
    }

    [Fact]
    public async Task UpdateAndDeleteCreditCard_ForAnotherUsersCard_ShouldReturnNotFound()
    {
        var ownerId = await SeedUserAsync();
        var otherUserId = await SeedUserAsync();

        SetCurrentUser(ownerId);
        Guid cardId;
        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();
            cardId = (await sender.Send(new CreateCreditCardCommand("Nubank", "Mastercard", 10))).Value;
        }

        SetCurrentUser(otherUserId);
        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();

            var updateResult = await sender.Send(new UpdateCreditCardCommand(cardId, "Hackeado", "Visa", 5));
            updateResult.IsFailure.Should().BeTrue();
            updateResult.Error.Should().Be(CreditCardErrors.NotFound);

            var deleteResult = await sender.Send(new DeleteCreditCardCommand(cardId));
            deleteResult.IsFailure.Should().BeTrue();
            deleteResult.Error.Should().Be(CreditCardErrors.NotFound);
        }
    }
}
