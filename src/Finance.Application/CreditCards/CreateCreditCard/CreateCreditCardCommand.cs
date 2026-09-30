using Finance.Application.Abstractions.Messaging;

namespace Finance.Application.CreditCards.CreateCreditCard;

public sealed record CreateCreditCardCommand(
    string Nickname,
    string Brand,
    int DueDay) : ICommand<Guid>;
