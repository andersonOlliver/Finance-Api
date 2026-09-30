using Finance.Application.Abstractions.Messaging;

namespace Finance.Application.Vehicles.SearchVehicles;

public sealed record SearchVehiclesQuery : IQuery<IReadOnlyList<VehicleResponse>>;
