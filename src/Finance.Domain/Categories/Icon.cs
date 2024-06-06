namespace Finance.Domain.Categories;

public record Icon(string Value)
{
    public static Icon Home => new("home");
    public static Icon Expense => new("arrow_downward");
    public static Icon Income => new("arrow_upward");
    public static Icon Transfer => new("swap_horiz");
    public static Icon Add => new("add");
    public static Icon Money => new("money");
    public static Icon Restaurant => new("restaurant");
    public static Icon Wallet => new("wallet");
}
