using Finance.Application.Abstractions.Messaging;

namespace Finance.Application.CreditCards.SearchCreditCards;

public sealed record SearchCreditCardsQuery : IQuery<IReadOnlyList<CreditCardResponse>>;
