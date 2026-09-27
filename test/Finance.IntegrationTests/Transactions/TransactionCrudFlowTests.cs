using Finance.Application.Transactions.CreateTransaction;
using Finance.Application.Transactions.DeleteTransaction;
using Finance.Application.Transactions.GetTransactionById;
using Finance.Application.Transactions.SearchTransactions;
using Finance.Application.Transactions.UpdateTransaction;
using Finance.Domain.Categories;
using Finance.Domain.Shared;
using Finance.Domain.Transactions;
using Finance.Domain.Users;
using Finance.Infrastructure;
using Finance.IntegrationTests.Infrastructure;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Finance.IntegrationTests.Transactions;

[Collection("Database")]
public class TransactionCrudFlowTests(DatabaseFixture fixture)
{
    private async Task<Guid> SeedCategoryAsync()
    {
        using var scope = fixture.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var categoryId = Guid.NewGuid();
        dbContext.Set<Category>().Add(Category.Create(
            categoryId, new Name("Alimentação"), CategoryType.Expense, Color.Orange, Icon.Restaurant, DateTime.UtcNow));

        await dbContext.SaveChangesWithoutEventsAsync();

        return categoryId;
    }

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

    private async Task<Guid> SeedCurrentUserAsync()
    {
        var userId = await SeedUserAsync();
        fixture.Services.GetRequiredService<TestUserContextAccessor>().UserId = userId;

        return userId;
    }

    [Fact]
    public async Task Should_create_search_update_and_delete_a_transaction()
    {
        await SeedCurrentUserAsync();
        var categoryId = await SeedCategoryAsync();
        var releasedOnUtc = DateTime.UtcNow;

        Guid transactionId;
        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();

            var createResult = await sender.Send(new CreateTransactionCommand(
                "Mercado", 150m, "USD", "Compras do mês", categoryId, null, releasedOnUtc));

            createResult.IsSuccess.Should().BeTrue();
            transactionId = createResult.Value;
        }

        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();

            var searchResult = await sender.Send(new SearchTransactionsQuery(null, null, null, null));

            searchResult.IsSuccess.Should().BeTrue();
            searchResult.Value.Should().ContainSingle(t =>
                t.Id == transactionId &&
                t.Amount == 150m &&
                t.CategoryName == "Alimentação");
        }

        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();

            var getResult = await sender.Send(new GetTransactionByIdQuery(transactionId));

            getResult.IsSuccess.Should().BeTrue();
            getResult.Value.Title.Should().Be("Mercado");
            getResult.Value.Amount.Should().Be(150m);
        }

        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();

            var updateResult = await sender.Send(new UpdateTransactionCommand(
                transactionId, "Mercado atualizado", 200m, "USD", null, categoryId, null, releasedOnUtc));

            updateResult.IsSuccess.Should().BeTrue();
        }

        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();

            var getResult = await sender.Send(new GetTransactionByIdQuery(transactionId));

            getResult.Value.Title.Should().Be("Mercado atualizado");
            getResult.Value.Amount.Should().Be(200m);
            getResult.Value.Description.Should().BeNull();
        }

        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();

            var deleteResult = await sender.Send(new DeleteTransactionCommand(transactionId));

            deleteResult.IsSuccess.Should().BeTrue();
        }

        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();

            var getResult = await sender.Send(new GetTransactionByIdQuery(transactionId));

            getResult.IsFailure.Should().BeTrue();
            getResult.Error.Should().Be(TransactionErrors.NotFound);
        }
    }

    [Fact]
    public async Task Should_not_expose_a_transaction_to_a_different_user()
    {
        var categoryId = await SeedCategoryAsync();

        Guid transactionId;
        await SeedCurrentUserAsync();
        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();

            var createResult = await sender.Send(new CreateTransactionCommand(
                "Aluguel", 1200m, "USD", null, categoryId, null, DateTime.UtcNow));

            transactionId = createResult.Value;
        }

        await SeedCurrentUserAsync();
        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();

            var getResult = await sender.Send(new GetTransactionByIdQuery(transactionId));

            getResult.IsFailure.Should().BeTrue();
            getResult.Error.Should().Be(TransactionErrors.NotFound);

            var searchResult = await sender.Send(new SearchTransactionsQuery(null, null, null, null));
            searchResult.Value.Should().NotContain(t => t.Id == transactionId);

            var deleteResult = await sender.Send(new DeleteTransactionCommand(transactionId));
            deleteResult.IsFailure.Should().BeTrue();
            deleteResult.Error.Should().Be(TransactionErrors.NotFound);
        }
    }
}
