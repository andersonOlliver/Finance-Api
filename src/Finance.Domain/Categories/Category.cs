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

    public Name Name { get; private set; }
    public CategoryType Type { get; private set; }
    public Color Color { get; private set; }
    public Icon Icon { get; private set; }
    public Guid? UserId { get; init; }
    public DateTime CreatedOnUtc { get; init; }
    public DateTime? UpdatedOnUtc { get; private set; }

    public static Category Create(Guid id, Name name, CategoryType type, Color color, Icon icon, DateTime createdOnUtc, Guid? userId = default)
    {
        return new Category(id, name, type, color, icon, userId, createdOnUtc);
    }

    public void Update(Name name, CategoryType type, Color color, Icon icon, DateTime updatedOnUtc)
    {
        Name = name;
        Type = type;
        Color = color;
        Icon = icon;
        UpdatedOnUtc = updatedOnUtc;
    }
}
