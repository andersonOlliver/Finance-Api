using Finance.Domain.Vehicles;
using Finance.Infrastructure;

namespace Finance.IntegrationTests.Infrastructure;

internal sealed class TestVehicleRefuelRepository(ApplicationDbContext dbContext) : IVehicleRefuelRepository
{
    public void Add(VehicleRefuel refuel) => dbContext.Add(refuel);
}
