using Finance.Application.Abstractions.Messaging;

namespace Finance.Application.Vehicles.CreateVehicleRefuel;

public sealed record CreateVehicleRefuelCommand(
    Guid VehicleId,
    decimal Liters,
    decimal PricePerLiter,
    string CurrencyCode,
    decimal Mileage,
    Guid CategoryId,
    Guid? PaymentId,
    string? Description,
    DateTime ReleasedOnUtc) : ICommand<VehicleRefuelResponse>;
