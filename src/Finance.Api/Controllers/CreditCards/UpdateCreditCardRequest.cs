namespace Finance.Api.Controllers.CreditCards;

public sealed record UpdateCreditCardRequest(string Nickname, string Brand, int DueDay);
