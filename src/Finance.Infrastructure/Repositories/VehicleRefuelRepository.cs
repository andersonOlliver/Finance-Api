using Finance.Domain.Vehicles;

namespace Finance.Infrastructure.Repositories;

internal sealed class VehicleRefuelRepository(ApplicationDbContext dbContext) : IVehicleRefuelRepository
{
    public void Add(VehicleRefuel refuel)
    {
        dbContext.Add(refuel);
    }
}
