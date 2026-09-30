using Finance.Application.Abstractions.Messaging;

namespace Finance.Application.CreditCards.DeleteCreditCard;

public sealed record DeleteCreditCardCommand(Guid Id) : ICommand;
