namespace Finance.Domain.Categories;

public interface ICategoryRepository
{
    void Add(Category category);
    void Remove(Category category);
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
