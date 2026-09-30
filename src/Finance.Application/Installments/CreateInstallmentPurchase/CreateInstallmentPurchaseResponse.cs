namespace Finance.Application.Installments.CreateInstallmentPurchase;

public sealed class CreateInstallmentPurchaseResponse
{
    public Guid Id { get; init; }
    public string? Title { get; init; }
    public decimal TotalAmount { get; init; }
    public int InstallmentCount { get; init; }
    public IReadOnlyList<GeneratedInstallmentResponse> Installments { get; init; } = [];
}

public sealed class GeneratedInstallmentResponse
{
    public Guid TransactionId { get; init; }
    public int InstallmentNumber { get; init; }
    public decimal Amount { get; init; }
    public DateTime ReleasedOnUtc { get; init; }
}
