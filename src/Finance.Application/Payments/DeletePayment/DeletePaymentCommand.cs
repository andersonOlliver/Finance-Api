using Finance.Application.Abstractions.Messaging;

namespace Finance.Application.Payments.DeletePayment;

public sealed record DeletePaymentCommand(Guid Id) : ICommand;
