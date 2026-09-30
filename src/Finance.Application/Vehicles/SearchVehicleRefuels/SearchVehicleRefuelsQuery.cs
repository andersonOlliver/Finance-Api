using Finance.Application.Abstractions.Messaging;

namespace Finance.Application.Vehicles.SearchVehicleRefuels;

public sealed record SearchVehicleRefuelsQuery(Guid VehicleId) : IQuery<IReadOnlyList<VehicleRefuelListItemResponse>>;
