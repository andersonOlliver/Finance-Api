namespace Finance.Application.Installments.SearchInstallmentPurchases;

public sealed class InstallmentPurchaseStatusResponse
{
    public Guid InstallmentPurchaseId { get; init; }
    public Guid TransactionId { get; init; }
    public string? Title { get; init; }
    public decimal TotalAmount { get; init; }
    public string? Currency { get; init; }
    public int InstallmentCount { get; init; }
    public int CurrentInstallmentNumber { get; init; }
    public decimal CurrentInstallmentAmount { get; init; }
    public DateTime ReleasedOnUtc { get; init; }
    public Guid CategoryId { get; init; }
    public string? CategoryName { get; init; }
    public Guid CreditCardId { get; init; }
    public string? CreditCardNickname { get; init; }
}
