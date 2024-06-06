using Finance.Domain.Abstracts;
using Finance.Domain.Shared;

namespace Finance.Domain.Categories;

public sealed class Category : Entity
{
    private Category(
        Guid id,
        Name name,
        CategoryType type,
        Color color,
        Icon icon,
        Guid? userId,
        DateTime createdOnUtc)
        : base(id)
    {
        Name = name;
        Type = type;
        Color = color;
        Icon = icon;
        UserId = userId;
        CreatedOnUtc = createdOnUtc;
    }

    private Category() { }

    public Name Name { get; init; }
    public CategoryType Type { get; init; }
    public Color Color { get; init; }
    public Icon Icon { get; init; }
    public Guid? UserId { get; init; }
    public DateTime CreatedOnUtc { get; init; }
    public DateTime? UpdatedOnUtc { get; init; }

    public static Category Create(Guid id, Name name, CategoryType type, Color color, Icon icon, DateTime createdOnUtc, Guid? userId = default)
    {
        return new Category(id, name, type, color, icon, userId, createdOnUtc);
    }
}
