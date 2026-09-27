using Finance.Domain.Abstracts;

namespace Finance.Domain.Transactions;

public static class TransactionErrors
{
    public static Error NotFound = new(
        "Transaction.NotFound",
        "O lançamento com o identificador informado não foi encontrado");
}
