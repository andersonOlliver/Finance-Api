using Finance.Domain.Vehicles;
using Finance.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Finance.IntegrationTests.Infrastructure;

internal sealed class TestVehicleRepository(ApplicationDbContext dbContext) : IVehicleRepository
{
    public void Add(Vehicle vehicle) => dbContext.Add(vehicle);

    public void Remove(Vehicle vehicle) => dbContext.Remove(vehicle);

    public Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Set<Vehicle>().FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
}
