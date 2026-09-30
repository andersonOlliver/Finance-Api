using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Abstracts;
using Finance.Domain.Vehicles;

namespace Finance.Application.Vehicles.DeleteVehicle;

internal sealed class DeleteVehicleCommandHandler(
    IVehicleRepository vehicleRepository,
    IUserContext userContext,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteVehicleCommand>
{
    public async Task<Result> Handle(DeleteVehicleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await vehicleRepository.GetByIdAsync(request.Id, cancellationToken);

        if (vehicle is null || vehicle.UserId != userContext.UserId)
        {
            return Result.Failure(VehicleErrors.NotFound);
        }

        vehicleRepository.Remove(vehicle);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
