using Finance.Domain.Payments;

namespace Finance.Application.Payments.SearchPayments;

public sealed class PaymentResponse
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public PaymentType Type { get; init; }
    public Guid? UserId { get; init; }
}
