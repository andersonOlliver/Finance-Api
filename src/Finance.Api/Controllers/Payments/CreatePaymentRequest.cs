using Finance.Domain.Payments;

namespace Finance.Api.Controllers.Payments;

public sealed record CreatePaymentRequest(string Name, PaymentType Type);
