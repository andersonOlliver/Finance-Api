namespace Finance.Api.Controllers.Transactions;

public sealed record CreateTransactionRequest(
    string Title,
    decimal Amount,
    string CurrencyCode,
    string? Description,
    Guid CategoryId,
    Guid? PaymentId,
    Guid? VehicleId,
    DateTime ReleasedOnUtc);
