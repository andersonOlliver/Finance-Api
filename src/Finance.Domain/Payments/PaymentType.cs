using System.ComponentModel;

namespace Finance.Domain.Payments;

public enum PaymentType
{
    [Description("Débito")]
    Debit = 1,

    [Description("Crédito à vista")]
    CashCredit = 2,
    
    [Description("Crédito parcelado")]
    InstallmentCredit = 3,
    
    [Description("Dinheiro")]
    Money = 4,

    [Description("Boleto")]
    Slip = 5,
}