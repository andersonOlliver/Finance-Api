using Finance.Domain.Abstracts;

namespace Finance.Domain.CreditCards;

public static class CreditCardErrors
{
    public static Error NotFound = new(
        "CreditCard.NotFound",
        "O cartão de crédito com o identificador informado não foi encontrado");
}
