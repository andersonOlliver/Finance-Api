namespace Finance.Api.Controllers.Vehicles;

public sealed record CreateVehicleRefuelRequest(
    decimal Liters,
    decimal PricePerLiter,
    string CurrencyCode,
    decimal Mileage,
    Guid CategoryId,
    Guid? PaymentId,
    string? Description,
    DateTime ReleasedOnUtc);
