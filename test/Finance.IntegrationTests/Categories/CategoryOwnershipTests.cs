using Finance.Application.Categories.CreateCategory;
using Finance.Application.Categories.DeleteCategory;
using Finance.Application.Categories.SearchCategories;
using Finance.Application.Categories.UpdateCategory;
using Finance.Domain.Categories;
using Finance.Domain.Shared;
using Finance.Domain.Users;
using Finance.Infrastructure;
using Finance.IntegrationTests.Infrastructure;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Finance.IntegrationTests.Categories;

[Collection("Database")]
public class CategoryOwnershipTests(DatabaseFixture fixture)
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

    private async Task<Guid> SeedDefaultCategoryAsync(string name)
    {
        using var scope = fixture.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var category = Category.Create(Guid.NewGuid(), new Name(name), CategoryType.Expense, Color.Orange, Icon.Restaurant, DateTime.UtcNow);
        dbContext.Set<Category>().Add(category);
        await dbContext.SaveChangesWithoutEventsAsync();

        return category.Id;
    }

    private void SetCurrentUser(Guid userId)
    {
        fixture.Services.GetRequiredService<TestUserContextAccessor>().UserId = userId;
    }

    [Fact]
    public async Task SearchCategories_ShouldReturnDefaultCategoriesAndOnlyTheCurrentUsersOwnCategories()
    {
        var defaultCategoryId = await SeedDefaultCategoryAsync($"Padrão-{Guid.NewGuid():N}");

        var userAId = await SeedUserAsync();
        var userBId = await SeedUserAsync();

        SetCurrentUser(userAId);
        Guid userACategoryId;
        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();
            var createResult = await sender.Send(new CreateCategoryCommand("Categoria da Usuária A", CategoryType.Expense, "F2994A", "wallet"));
            userACategoryId = createResult.Value;
        }

        SetCurrentUser(userBId);
        Guid userBCategoryId;
        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();
            var createResult = await sender.Send(new CreateCategoryCommand("Categoria do Usuário B", CategoryType.Expense, "F2994A", "wallet"));
            userBCategoryId = createResult.Value;
        }

        SetCurrentUser(userAId);
        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();
            var result = await sender.Send(new SearchCategoryQuery());

            result.Value.Should().Contain(c => c.Id == defaultCategoryId);
            result.Value.Should().Contain(c => c.Id == userACategoryId);
            result.Value.Should().NotContain(c => c.Id == userBCategoryId);
        }
    }

    [Fact]
    public async Task Categories_WithSameName_FromDifferentUsers_ShouldBeTreatedAsDistinctEntities()
    {
        var userAId = await SeedUserAsync();
        var userBId = await SeedUserAsync();
        const string sharedName = "Alimentação";

        SetCurrentUser(userAId);
        Guid categoryAId;
        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();
            categoryAId = (await sender.Send(new CreateCategoryCommand(sharedName, CategoryType.Expense, "F2994A", "restaurant"))).Value;
        }

        SetCurrentUser(userBId);
        Guid categoryBId;
        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();
            categoryBId = (await sender.Send(new CreateCategoryCommand(sharedName, CategoryType.Expense, "F2994A", "restaurant"))).Value;
        }

        categoryAId.Should().NotBe(categoryBId);

        SetCurrentUser(userAId);
        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();

            var updateResult = await sender.Send(new UpdateCategoryCommand(categoryAId, "Alimentação e mercado", CategoryType.Expense, "F2994A", "restaurant"));
            updateResult.IsSuccess.Should().BeTrue();

            var updateOthersResult = await sender.Send(new UpdateCategoryCommand(categoryBId, "Hackeado", CategoryType.Expense, "F2994A", "restaurant"));
            updateOthersResult.IsFailure.Should().BeTrue();
        }
    }

    [Fact]
    public async Task DeleteCategory_ForADefaultCategory_ShouldReturnNotFound()
    {
        var defaultCategoryId = await SeedDefaultCategoryAsync($"Padrão-{Guid.NewGuid():N}");
        var userId = await SeedUserAsync();

        SetCurrentUser(userId);
        using var scope = fixture.Services.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        var result = await sender.Send(new DeleteCategoryCommand(defaultCategoryId));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CategoryErrors.NotFound);
    }
}
