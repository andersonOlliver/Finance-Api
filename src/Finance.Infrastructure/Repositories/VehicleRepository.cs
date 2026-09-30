using Finance.Domain.Vehicles;

namespace Finance.Infrastructure.Repositories;

internal sealed class VehicleRepository(ApplicationDbContext context) : Repository<Vehicle>(context), IVehicleRepository
{
}
