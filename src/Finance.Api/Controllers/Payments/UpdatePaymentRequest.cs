using Finance.Domain.Payments;

namespace Finance.Api.Controllers.Payments;

public sealed record UpdatePaymentRequest(string Name, PaymentType Type);
