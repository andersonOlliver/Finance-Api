using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Payments;

namespace Finance.Application.Payments.UpdatePayment;

public sealed record UpdatePaymentCommand(Guid Id, string Name, PaymentType Type) : ICommand;
