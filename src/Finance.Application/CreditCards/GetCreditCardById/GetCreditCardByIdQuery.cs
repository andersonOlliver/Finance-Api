using Finance.Application.Abstractions.Messaging;
using Finance.Application.CreditCards.SearchCreditCards;

namespace Finance.Application.CreditCards.GetCreditCardById;

public sealed record GetCreditCardByIdQuery(Guid Id) : IQuery<CreditCardResponse>;
