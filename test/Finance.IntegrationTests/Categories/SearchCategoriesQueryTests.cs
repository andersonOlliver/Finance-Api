using Finance.Application.Categories.SearchCategories;
using Finance.Domain.Categories;
using Finance.Domain.Shared;
using Finance.Infrastructure;
using Finance.IntegrationTests.Infrastructure;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Finance.IntegrationTests.Categories;

[Collection("Database")]
public class SearchCategoriesQueryTests(DatabaseFixture fixture)
{
    [Fact]
    public async Task Should_return_seeded_categories()
    {
        var categoryId = Guid.NewGuid();
        using (var scope = fixture.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Set<Category>().Add(Category.Create(
                categoryId, new Name("Salário"), CategoryType.Receive, Color.Green, Icon.Money, DateTime.UtcNow));
            await dbContext.SaveChangesWithoutEventsAsync();
        }

        using var queryScope = fixture.Services.CreateScope();
        var sender = queryScope.ServiceProvider.GetRequiredService<ISender>();

        var result = await sender.Send(new SearchCategoryQuery());

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Contain(c => c.Id == categoryId && c.Name == "Salário" && c.Type == CategoryType.Receive);
    }
}
