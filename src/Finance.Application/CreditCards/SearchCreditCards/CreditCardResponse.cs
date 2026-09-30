namespace Finance.Application.CreditCards.SearchCreditCards;

public sealed class CreditCardResponse
{
    public Guid Id { get; init; }
    public string? Nickname { get; init; }
    public string? Brand { get; init; }
    public int DueDay { get; init; }
    public DateTime CreatedOnUtc { get; init; }
    public DateTime? UpdatedOnUtc { get; init; }
}
