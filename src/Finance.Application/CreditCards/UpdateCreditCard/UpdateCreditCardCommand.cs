using Finance.Application.Abstractions.Messaging;

namespace Finance.Application.CreditCards.UpdateCreditCard;

public sealed record UpdateCreditCardCommand(
    Guid Id,
    string Nickname,
    string Brand,
    int DueDay) : ICommand;
