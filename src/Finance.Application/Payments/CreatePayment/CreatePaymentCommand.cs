using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Payments;

namespace Finance.Application.Payments.CreatePayment;

public sealed record CreatePaymentCommand(string Name, PaymentType Type) : ICommand<Guid>;
