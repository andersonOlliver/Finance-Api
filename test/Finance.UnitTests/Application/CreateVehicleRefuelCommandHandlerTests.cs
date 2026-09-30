using Finance.Application.Exceptions;
using Finance.Application.Vehicles.CreateVehicleRefuel;
using Finance.Domain.Shared;
using Finance.Domain.Transactions;
using Finance.Domain.Vehicles;
using FluentAssertions;
using NSubstitute;

namespace Finance.UnitTests.Application;

public class CreateVehicleRefuelCommandHandlerTests
{
    private readonly ApplicationTestHarness _harness = new();

    private static Vehicle CreateExistingVehicle(Guid userId, decimal mileage) =>
        Vehicle.Create(Guid.NewGuid(), new Name("Meu Civic"), VehicleType.Car, "Honda", "Civic", "Prata", 2020, "ABC1D23", mileage, userId, DateTime.UtcNow);

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateTransactionUpdateMileageAndComputeConsumption()
    {
        var userId = Guid.NewGuid();
        var vehicle = CreateExistingVehicle(userId, 15000m);
        var categoryId = Guid.NewGuid();

        _harness.UserContext.UserId.Returns(userId);
        _harness.VehicleRepository.GetByIdAsync(vehicle.Id, Arg.Any<CancellationToken>()).Returns(vehicle);

        var command = new CreateVehicleRefuelCommand(
            vehicle.Id, 40m, 5m, "USD", 15400m, categoryId, null, "Posto Ipiranga", DateTime.UtcNow);

        var result = await _harness.Sender.Send(command);

        result.IsSuccess.Should().BeTrue();
        result.Value.Amount.Should().Be(200m);
        result.Value.DistanceSinceLastRefuel.Should().Be(400m);
        result.Value.ConsumptionSinceLastRefuel.Should().Be(10m);

        vehicle.Mileage.Should().Be(15400m);

        _harness.TransactionRepository.Received(1).Add(Arg.Is<Transaction>(t =>
            t.VehicleId == vehicle.Id &&
            t.CategoryId == categoryId &&
            t.Value.Amount == 200m &&
            t.UserId == userId));

        _harness.VehicleRefuelRepository.Received(1).Add(Arg.Is<VehicleRefuel>(r =>
            r.VehicleId == vehicle.Id &&
            r.Liters == 40m &&
            r.Mileage == 15400m &&
            r.ConsumptionSinceLastRefuel == 10m));

        await _harness.UnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenMileageIsNotGreaterThanCurrent_ShouldReturnFailure()
    {
        var userId = Guid.NewGuid();
        var vehicle = CreateExistingVehicle(userId, 15000m);

        _harness.UserContext.UserId.Returns(userId);
        _harness.VehicleRepository.GetByIdAsync(vehicle.Id, Arg.Any<CancellationToken>()).Returns(vehicle);

        var command = new CreateVehicleRefuelCommand(
            vehicle.Id, 40m, 5m, "USD", 15000m, Guid.NewGuid(), null, null, DateTime.UtcNow);

        var result = await _harness.Sender.Send(command);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(VehicleErrors.MileageMustBeGreaterThanCurrent);

        _harness.TransactionRepository.DidNotReceive().Add(Arg.Any<Transaction>());
        _harness.VehicleRefuelRepository.DidNotReceive().Add(Arg.Any<VehicleRefuel>());
    }

    [Fact]
    public async Task Handle_WhenVehicleBelongsToAnotherUser_ShouldReturnNotFound()
    {
        var vehicle = CreateExistingVehicle(Guid.NewGuid(), 15000m);

        _harness.UserContext.UserId.Returns(Guid.NewGuid());
        _harness.VehicleRepository.GetByIdAsync(vehicle.Id, Arg.Any<CancellationToken>()).Returns(vehicle);

        var command = new CreateVehicleRefuelCommand(
            vehicle.Id, 40m, 5m, "USD", 15400m, Guid.NewGuid(), null, null, DateTime.UtcNow);

        var result = await _harness.Sender.Send(command);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(VehicleErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WithNonPositiveLiters_ShouldThrowValidationException()
    {
        var command = new CreateVehicleRefuelCommand(
            Guid.NewGuid(), 0m, 5m, "USD", 15400m, Guid.NewGuid(), null, null, DateTime.UtcNow);

        var act = () => _harness.Sender.Send(command);

        await act.Should().ThrowAsync<ValidationException>();
        _harness.VehicleRepository.DidNotReceiveWithAnyArgs().GetByIdAsync(default, default);
    }
}
