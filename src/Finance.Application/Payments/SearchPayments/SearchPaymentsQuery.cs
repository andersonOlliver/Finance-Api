using Finance.Application.Abstractions.Messaging;

namespace Finance.Application.Payments.SearchPayments;

public sealed record SearchPaymentsQuery : IQuery<IReadOnlyList<PaymentResponse>>;
