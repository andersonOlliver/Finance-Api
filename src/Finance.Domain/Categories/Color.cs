namespace Finance.Domain.Categories;

public record Color(string Value)
{
    public static Color Red => new("e04f5f");
    public static Color RedAcent => new("EB5757");
    public static Color Orange => new("F2994A");
    public static Color Green => new("219653");
    public static Color GreenLight => new("219653");
}
