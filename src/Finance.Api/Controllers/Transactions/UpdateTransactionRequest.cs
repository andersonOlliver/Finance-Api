namespace Finance.Api.Controllers.Transactions;

public sealed record UpdateTransactionRequest(
    string Title,
    decimal Amount,
    string CurrencyCode,
    string? Description,
    Guid CategoryId,
    Guid? PaymentId,
    Guid? VehicleId,
    DateTime ReleasedOnUtc);
