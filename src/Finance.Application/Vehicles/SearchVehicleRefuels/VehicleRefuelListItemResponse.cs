namespace Finance.Application.Vehicles.SearchVehicleRefuels;

public sealed class VehicleRefuelListItemResponse
{
    public Guid Id { get; init; }
    public Guid VehicleId { get; init; }
    public Guid TransactionId { get; init; }
    public decimal Liters { get; init; }
    public decimal PricePerLiter { get; init; }
    public decimal Amount { get; init; }
    public decimal Mileage { get; init; }
    public decimal? ConsumptionSinceLastRefuel { get; init; }
    public DateTime RefueledOnUtc { get; init; }
}
