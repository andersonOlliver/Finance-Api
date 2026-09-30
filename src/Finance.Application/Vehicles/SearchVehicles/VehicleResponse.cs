using Finance.Domain.Vehicles;

namespace Finance.Application.Vehicles.SearchVehicles;

public sealed class VehicleResponse
{
    public Guid Id { get; init; }
    public string? Nickname { get; init; }
    public VehicleType Type { get; init; }
    public string? Brand { get; init; }
    public string? Model { get; init; }
    public string? Color { get; init; }
    public int Year { get; init; }
    public string? LicensePlate { get; init; }
    public decimal Mileage { get; init; }
    public DateTime CreatedOnUtc { get; init; }
    public DateTime? UpdatedOnUtc { get; init; }
}
