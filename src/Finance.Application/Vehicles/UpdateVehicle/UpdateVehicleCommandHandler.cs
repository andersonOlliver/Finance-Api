using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Clock;
using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Abstracts;
using Finance.Domain.Shared;
using Finance.Domain.Vehicles;

namespace Finance.Application.Vehicles.UpdateVehicle;

internal sealed class UpdateVehicleCommandHandler(
    IVehicleRepository vehicleRepository,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateVehicleCommand>
{
    public async Task<Result> Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await vehicleRepository.GetByIdAsync(request.Id, cancellationToken);

        if (vehicle is null || vehicle.UserId != userContext.UserId)
        {
            return Result.Failure(VehicleErrors.NotFound);
        }

        vehicle.Update(
            new Name(request.Nickname),
            request.Type,
            request.Brand,
            request.Model,
            request.Color,
            request.Year,
            request.LicensePlate,
            dateTimeProvider.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
