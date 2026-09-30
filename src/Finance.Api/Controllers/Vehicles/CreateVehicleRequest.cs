using Finance.Domain.Vehicles;

namespace Finance.Api.Controllers.Vehicles;

public sealed record CreateVehicleRequest(
    string Nickname,
    VehicleType Type,
    string Brand,
    string Model,
    string Color,
    int Year,
    string LicensePlate,
    decimal InitialMileage);
