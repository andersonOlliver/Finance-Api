using Finance.Domain.Categories;
using Finance.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Finance.IntegrationTests.Infrastructure;

internal sealed class TestCategoryRepository(ApplicationDbContext dbContext) : ICategoryRepository
{
    public void Add(Category category) => dbContext.Add(category);

    public void Remove(Category category) => dbContext.Remove(category);

    public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Set<Category>().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
}
