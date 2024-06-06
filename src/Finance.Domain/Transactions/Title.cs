using Finance.Domain.Abstracts;

namespace Finance.Domain.Transactions;

public record Title
{
    public static readonly Error MaxLenghtError = new("Title.MaxLenght", $"The title should have less then {MaxLenght} characters");
    public static readonly Error NullError = new("Title.NullError", $"The title should be informed");

    public const int MaxLenght = 100;

    private Title(string value)
    {
        Value = value;
    }


    public string Value { get; init; }

    public static Result<Title> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<Title>(MaxLenghtError);

        if (value.Length > MaxLenght)
            return Result.Failure<Title>(NullError);

        return new Title(value);
    }

    public static implicit operator string(Title title) => title.Value;
}
