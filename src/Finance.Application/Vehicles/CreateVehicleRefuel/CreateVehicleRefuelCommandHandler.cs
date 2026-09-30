using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Clock;
using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Abstracts;
using Finance.Domain.Transactions;
using Finance.Domain.Vehicles;

namespace Finance.Application.Vehicles.CreateVehicleRefuel;

internal sealed class CreateVehicleRefuelCommandHandler(
    IVehicleRepository vehicleRepository,
    IVehicleRefuelRepository vehicleRefuelRepository,
    ITransactionRepository transactionRepository,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateVehicleRefuelCommand, VehicleRefuelResponse>
{
    public async Task<Result<VehicleRefuelResponse>> Handle(CreateVehicleRefuelCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await vehicleRepository.GetByIdAsync(request.VehicleId, cancellationToken);

        if (vehicle is null || vehicle.UserId != userContext.UserId)
        {
            return Result.Failure<VehicleRefuelResponse>(VehicleErrors.NotFound);
        }

        if (request.Mileage <= vehicle.Mileage)
        {
            return Result.Failure<VehicleRefuelResponse>(VehicleErrors.MileageMustBeGreaterThanCurrent);
        }

        var now = dateTimeProvider.UtcNow;
        var currency = Currency.FromCode(request.CurrencyCode);
        var amount = new Money(request.Liters * request.PricePerLiter, currency);
        var title = Title.Create("Abastecimento").Value;
        var description = request.Description is null ? null : new Description(request.Description);

        var transaction = Transaction.Create(
            Guid.CreateVersion7(),
            title,
            amount,
            description,
            userContext.UserId,
            request.CategoryId,
            request.PaymentId,
            vehicle.Id,
            request.ReleasedOnUtc,
            now);

        transactionRepository.Add(transaction);

        var distanceSinceLastRefuel = request.Mileage - vehicle.Mileage;
        var consumption = distanceSinceLastRefuel / request.Liters;

        var refuel = VehicleRefuel.Create(
            Guid.CreateVersion7(),
            vehicle.Id,
            transaction.Id,
            request.Liters,
            request.PricePerLiter,
            request.Mileage,
            consumption,
            request.ReleasedOnUtc,
            now);

        vehicleRefuelRepository.Add(refuel);

        vehicle.UpdateMileage(request.Mileage, now);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new VehicleRefuelResponse
        {
            Id = refuel.Id,
            VehicleId = vehicle.Id,
            TransactionId = transaction.Id,
            Liters = refuel.Liters,
            PricePerLiter = refuel.PricePerLiter,
            Amount = amount.Amount,
            Mileage = refuel.Mileage,
            DistanceSinceLastRefuel = distanceSinceLastRefuel,
            ConsumptionSinceLastRefuel = refuel.ConsumptionSinceLastRefuel,
            RefueledOnUtc = refuel.RefueledOnUtc
        };
    }
}
