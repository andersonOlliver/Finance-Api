using Finance.Domain.Categories;
using Finance.Domain.Payments;

namespace Finance.Application.Transactions.SearchTransactions;

public sealed class TransactionResponse
{
    public Guid Id { get; init; }
    public string? Title { get; init; }
    public decimal Amount { get; init; }
    public string? Currency { get; init; }
    public string? Description { get; init; }
    public Guid CategoryId { get; init; }
    public string? CategoryName { get; init; }
    public CategoryType CategoryType { get; init; }
    public string? CategoryColor { get; init; }
    public string? CategoryIcon { get; init; }
    public Guid? PaymentId { get; init; }
    public string? PaymentName { get; init; }
    public PaymentType? PaymentType { get; init; }
    public Guid? VehicleId { get; init; }
    public string? VehicleNickname { get; init; }
    public DateTime ReleasedOnUtc { get; init; }
    public DateTime CreatedOnUtc { get; init; }
    public DateTime? UpdatedOnUtc { get; init; }
}
