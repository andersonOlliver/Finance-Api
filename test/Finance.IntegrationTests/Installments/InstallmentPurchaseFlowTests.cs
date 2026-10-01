using Finance.Application.Installments.CreateInstallmentPurchase;
using Finance.Application.Installments.DeleteInstallmentPurchase;
using Finance.Application.Installments.GetInstallmentPurchaseById;
using Finance.Application.Installments.SearchInstallmentPurchases;
using Finance.Application.Transactions.GetTransactionById;
using Finance.Domain.Categories;
using Finance.Domain.CreditCards;
using Finance.Domain.Installments;
using Finance.Domain.Shared;
using Finance.Domain.Transactions;
using Finance.Domain.Users;
using Finance.Infrastructure;
using Finance.IntegrationTests.Infrastructure;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Finance.IntegrationTests.Installments;

[Collection("Database")]
public class InstallmentPurchaseFlowTests(DatabaseFixture fixture)
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
        user.SetIdentityId(Guid.NewGuid().ToString());

        dbContext.Set<User>().Add(user);
        await dbContext.SaveChangesWithoutEventsAsync();

        return user.Id;
    }

    private async Task<Guid> SeedCategoryAsync()
    {
        using var scope = fixture.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var category = Category.Create(Guid.NewGuid(), new Name("Eletrônicos"), CategoryType.Expense, Color.Orange, Icon.Wallet, DateTime.UtcNow);
        dbContext.Set<Category>().Add(category);
        await dbContext.SaveChangesWithoutEventsAsync();

        return category.Id;
    }

    private async Task<Guid> SeedCreditCardAsync(Guid userId, int dueDay)
    {
        using var scope = fixture.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var card = CreditCard.Create(Guid.NewGuid(), new Name("Nubank"), "Mastercard", dueDay, userId, DateTime.UtcNow);
        dbContext.Set<CreditCard>().Add(card);
        await dbContext.SaveChangesWithoutEventsAsync();

        return card.Id;
    }

    /// <summary>
    /// Seeds an installment purchase and its generated transactions directly, bypassing
    /// CreateInstallmentPurchaseCommand's "first installment can't be in the past" rule, so
    /// historical scenarios (like the ones used to validate month-based querying) can be set up.
    /// </summary>
    private async Task<(Guid PurchaseId, Guid[] TransactionIds)> SeedInstallmentPurchaseAsync(
        Guid userId, Guid categoryId, Guid creditCardId, string title, decimal totalAmount, int installmentCount, DateTime firstInstallmentReleasedOnUtc)
    {
        using var scope = fixture.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var installmentAmount = Math.Round(totalAmount / installmentCount, 2, MidpointRounding.AwayFromZero);

        var purchase = InstallmentPurchase.Create(
            Guid.NewGuid(),
            Title.Create(title).Value,
            new Money(totalAmount, Currency.Usd),
            installmentCount,
            userId,
            categoryId,
            creditCardId,
            firstInstallmentReleasedOnUtc,
            DateTime.UtcNow);

        dbContext.Set<InstallmentPurchase>().Add(purchase);

        var transactionIds = new Guid[installmentCount];

        for (var i = 0; i < installmentCount; i++)
        {
            var transaction = Transaction.Create(
                Guid.NewGuid(),
                Title.Create($"{title} ({i + 1}/{installmentCount})").Value,
                new Money(installmentAmount, Currency.Usd),
                null,
                userId,
                categoryId,
                null,
                null,
                firstInstallmentReleasedOnUtc.AddMonths(i),
                DateTime.UtcNow,
                purchase.Id,
                i + 1,
                creditCardId);

            transactionIds[i] = transaction.Id;
            dbContext.Set<Transaction>().Add(transaction);
        }

        await dbContext.SaveChangesWithoutEventsAsync();

        return (purchase.Id, transactionIds);
    }

    private void SetCurrentUser(Guid userId)
    {
        fixture.Services.GetRequiredService<TestUserContextAccessor>().UserId = userId;
    }

    [Fact]
    public async Task SearchInstallmentPurchases_ForTheReferenceMonth_ShouldShowEachPurchasesCurrentInstallment()
    {
        // Reproduces the exact scenario described when requesting this feature: a purchase whose
        // first installment lands in February 2026 (12x) and another whose first installment lands
        // in June 2026 (10x) should show up as 5/12 and 1/10 respectively when queried in June 2026.
        var userId = await SeedUserAsync();
        SetCurrentUser(userId);
        var categoryId = await SeedCategoryAsync();
        var creditCardId = await SeedCreditCardAsync(userId, 10);

        await SeedInstallmentPurchaseAsync(
            userId, categoryId, creditCardId, "Notebook Dell", 1200m, 12,
            new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc));

        await SeedInstallmentPurchaseAsync(
            userId, categoryId, creditCardId, "Geladeira", 1000m, 10,
            new DateTime(2026, 6, 10, 0, 0, 0, DateTimeKind.Utc));

        using var scope = fixture.Services.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        var result = await sender.Send(new SearchInstallmentPurchasesQuery(new DateTime(2026, 6, 15), null));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);

        result.Value.Should().ContainSingle(p => p.Title == "Notebook Dell" && p.CurrentInstallmentNumber == 5 && p.InstallmentCount == 12);
        result.Value.Should().ContainSingle(p => p.Title == "Geladeira" && p.CurrentInstallmentNumber == 1 && p.InstallmentCount == 10);
    }

    [Fact]
    public async Task SearchInstallmentPurchases_FilteredByCreditCard_ShouldOnlyReturnThatCardsPurchases()
    {
        var userId = await SeedUserAsync();
        SetCurrentUser(userId);
        var categoryId = await SeedCategoryAsync();
        var cardAId = await SeedCreditCardAsync(userId, 10);
        var cardBId = await SeedCreditCardAsync(userId, 15);

        var referenceMonth = new DateTime(2026, 6, 15);

        await SeedInstallmentPurchaseAsync(userId, categoryId, cardAId, "Notebook Dell", 1200m, 12, new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc));
        await SeedInstallmentPurchaseAsync(userId, categoryId, cardBId, "Geladeira", 1000m, 10, new DateTime(2026, 6, 10, 0, 0, 0, DateTimeKind.Utc));

        using var scope = fixture.Services.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        var result = await sender.Send(new SearchInstallmentPurchasesQuery(referenceMonth, cardAId));

        result.Value.Should().ContainSingle();
        result.Value[0].Title.Should().Be("Notebook Dell");
    }

    [Fact]
    public async Task CreateInstallmentPurchase_ThroughTheFullCommand_ShouldPersistPlanAndAllInstallments()
    {
        var userId = await SeedUserAsync();
        SetCurrentUser(userId);
        var categoryId = await SeedCategoryAsync();
        var cardId = await SeedCreditCardAsync(userId, 20);

        Guid purchaseId;
        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();

            var createResult = await sender.Send(new CreateInstallmentPurchaseCommand(
                "Notebook Dell", 300m, "USD", 3, categoryId, cardId, "Compra parcelada", null));

            createResult.IsSuccess.Should().BeTrue();
            createResult.Value.Installments.Should().HaveCount(3);
            purchaseId = createResult.Value.Id;
        }

        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();

            var detailResult = await sender.Send(new GetInstallmentPurchaseByIdQuery(purchaseId));

            detailResult.IsSuccess.Should().BeTrue();
            detailResult.Value.CreditCardId.Should().Be(cardId);
            detailResult.Value.Installments.Should().HaveCount(3);
            detailResult.Value.Installments.Should().OnlyHaveUniqueItems(i => i.InstallmentNumber);
        }
    }

    [Fact]
    public async Task DeleteInstallmentPurchase_ShouldCascadeDeleteItsGeneratedTransactions()
    {
        var userId = await SeedUserAsync();
        SetCurrentUser(userId);
        var categoryId = await SeedCategoryAsync();
        var cardId = await SeedCreditCardAsync(userId, 20);

        var (purchaseId, transactionIds) = await SeedInstallmentPurchaseAsync(
            userId, categoryId, cardId, "Notebook Dell", 300m, 3, new DateTime(2026, 6, 20, 0, 0, 0, DateTimeKind.Utc));

        using var scope = fixture.Services.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        var deleteResult = await sender.Send(new DeleteInstallmentPurchaseCommand(purchaseId));
        deleteResult.IsSuccess.Should().BeTrue();

        var transactionResult = await sender.Send(new GetTransactionByIdQuery(transactionIds[0]));
        transactionResult.IsFailure.Should().BeTrue();
        transactionResult.Error.Should().Be(TransactionErrors.NotFound);
    }
}
