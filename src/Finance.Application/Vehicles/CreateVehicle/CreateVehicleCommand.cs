using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Vehicles;

namespace Finance.Application.Vehicles.CreateVehicle;

public sealed record CreateVehicleCommand(
    string Nickname,
    VehicleType Type,
    string Brand,
    string Model,
    string Color,
    int Year,
    string LicensePlate,
    decimal InitialMileage) : ICommand<Guid>;
