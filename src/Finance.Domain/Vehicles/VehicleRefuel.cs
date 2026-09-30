using Finance.Domain.Abstracts;

namespace Finance.Domain.Vehicles;

public sealed class VehicleRefuel : Entity
{
    private VehicleRefuel(
        Guid id,
        Guid vehicleId,
        Guid transactionId,
        decimal liters,
        decimal pricePerLiter,
        decimal mileage,
        decimal? consumptionSinceLastRefuel,
        DateTime refueledOnUtc,
        DateTime createdOnUtc)
        : base(id)
    {
        VehicleId = vehicleId;
        TransactionId = transactionId;
        Liters = liters;
        PricePerLiter = pricePerLiter;
        Mileage = mileage;
        ConsumptionSinceLastRefuel = consumptionSinceLastRefuel;
        RefueledOnUtc = refueledOnUtc;
        CreatedOnUtc = createdOnUtc;
    }

    private VehicleRefuel() { }

    public Guid VehicleId { get; init; }
    public Guid TransactionId { get; init; }
    public decimal Liters { get; init; }
    public decimal PricePerLiter { get; init; }
    public decimal Mileage { get; init; }
    public decimal? ConsumptionSinceLastRefuel { get; init; }
    public DateTime RefueledOnUtc { get; init; }
    public DateTime CreatedOnUtc { get; init; }

    public static VehicleRefuel Create(
        Guid id,
        Guid vehicleId,
        Guid transactionId,
        decimal liters,
        decimal pricePerLiter,
        decimal mileage,
        decimal? consumptionSinceLastRefuel,
        DateTime refueledOnUtc,
        DateTime createdOnUtc)
    {
        return new VehicleRefuel(id, vehicleId, transactionId, liters, pricePerLiter, mileage, consumptionSinceLastRefuel, refueledOnUtc, createdOnUtc);
    }
}
