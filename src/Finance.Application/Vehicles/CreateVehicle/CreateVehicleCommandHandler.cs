using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Clock;
using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Abstracts;
using Finance.Domain.Shared;
using Finance.Domain.Vehicles;

namespace Finance.Application.Vehicles.CreateVehicle;

internal sealed class CreateVehicleCommandHandler(
    IVehicleRepository vehicleRepository,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateVehicleCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = Vehicle.Create(
            Guid.CreateVersion7(),
            new Name(request.Nickname),
            request.Type,
            request.Brand,
            request.Model,
            request.Color,
            request.Year,
            request.LicensePlate,
            request.InitialMileage,
            userContext.UserId,
            dateTimeProvider.UtcNow);

        vehicleRepository.Add(vehicle);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return vehicle.Id;
    }
}
