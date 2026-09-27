using Finance.Domain.Abstracts;

namespace Finance.Domain.Payments;

public static class PaymentErrors
{
    public static Error NotFound = new(
        "Payment.NotFound",
        "A forma de pagamento com o identificador informado não foi encontrada");
}
