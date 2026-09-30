namespace Finance.Application.Installments.GetInstallmentPurchaseById;

public sealed class InstallmentPurchaseDetailResponse
{
    public Guid Id { get; init; }
    public string? Title { get; init; }
    public decimal TotalAmount { get; init; }
    public string? Currency { get; init; }
    public int InstallmentCount { get; init; }
    public Guid CategoryId { get; init; }
    public string? CategoryName { get; init; }
    public Guid CreditCardId { get; init; }
    public string? CreditCardNickname { get; init; }
    public DateTime FirstInstallmentReleasedOnUtc { get; init; }
    public DateTime CreatedOnUtc { get; init; }
    public IReadOnlyList<InstallmentLineResponse> Installments { get; init; } = [];
}

public sealed class InstallmentLineResponse
{
    public Guid TransactionId { get; init; }
    public int InstallmentNumber { get; init; }
    public decimal Amount { get; init; }
    public DateTime ReleasedOnUtc { get; init; }
}
