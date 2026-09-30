using Finance.Application.Abstractions.Messaging;
using Finance.Application.Vehicles.SearchVehicles;

namespace Finance.Application.Vehicles.GetVehicleById;

public sealed record GetVehicleByIdQuery(Guid Id) : IQuery<VehicleResponse>;
