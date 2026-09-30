namespace Finance.Api.Controllers.CreditCards;

public sealed record CreateCreditCardRequest(string Nickname, string Brand, int DueDay);
