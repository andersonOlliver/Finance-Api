namespace Finance.Domain.Vehicles;

public interface IVehicleRepository
{
    void Add(Vehicle vehicle);
    void Remove(Vehicle vehicle);
    Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
