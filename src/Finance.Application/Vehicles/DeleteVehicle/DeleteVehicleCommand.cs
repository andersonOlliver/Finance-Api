using Finance.Application.Abstractions.Messaging;

namespace Finance.Application.Vehicles.DeleteVehicle;

public sealed record DeleteVehicleCommand(Guid Id) : ICommand;
