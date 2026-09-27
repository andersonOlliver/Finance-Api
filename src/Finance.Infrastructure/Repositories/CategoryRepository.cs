using Finance.Domain.Categories;

namespace Finance.Infrastructure.Repositories;

internal sealed class CategoryRepository(ApplicationDbContext context) : Repository<Category>(context), ICategoryRepository
{
}
