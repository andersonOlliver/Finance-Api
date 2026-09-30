using Finance.Domain.Abstracts;

namespace Finance.Domain.Installments;

public static class InstallmentPurchaseErrors
{
    public static Error NotFound = new(
        "InstallmentPurchase.NotFound",
        "O parcelamento com o identificador informado não foi encontrado");

    public static Error CreditCardNotFound = new(
        "InstallmentPurchase.CreditCardNotFound",
        "O cartão de crédito informado não foi encontrado");

    public static Error FirstInstallmentMustBeOnOrAfterNextDueDate = new(
        "InstallmentPurchase.FirstInstallmentMustBeOnOrAfterNextDueDate",
        "A data da primeira parcela deve ser igual ou posterior ao próximo vencimento do cartão");
}
