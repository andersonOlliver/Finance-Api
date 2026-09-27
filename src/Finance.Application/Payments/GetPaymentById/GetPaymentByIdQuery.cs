using Finance.Application.Abstractions.Messaging;
using Finance.Application.Payments.SearchPayments;

namespace Finance.Application.Payments.GetPaymentById;

public sealed record GetPaymentByIdQuery(Guid Id) : IQuery<PaymentResponse>;
