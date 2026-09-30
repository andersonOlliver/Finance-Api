using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Vehicles;

namespace Finance.Application.Vehicles.UpdateVehicle;

public sealed record UpdateVehicleCommand(
    Guid Id,
    string Nickname,
    VehicleType Type,
    string Brand,
    string Model,
    string Color,
    int Year,
    string LicensePlate) : ICommand;
