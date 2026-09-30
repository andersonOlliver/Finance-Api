using Finance.Application.Vehicles.DeleteVehicle;
using Finance.Domain.Shared;
using Finance.Domain.Vehicles;
using FluentAssertions;
using NSubstitute;

namespace Finance.UnitTests.Application;

public class DeleteVehicleCommandHandlerTests
{
    private readonly ApplicationTestHarness _harness = new();

    private static Vehicle CreateExistingVehicle(Guid userId) =>
        Vehicle.Create(Guid.NewGuid(), new Name("Meu Civic"), VehicleType.Car, "Honda", "Civic", "Prata", 2020, "ABC1D23", 15000m, userId, DateTime.UtcNow);

    [Fact]
    public async Task Handle_WithOwnedVehicle_ShouldRemoveItAndSucceed()
    {
        var userId = Guid.NewGuid();
        var vehicle = CreateExistingVehicle(userId);

        _harness.UserContext.UserId.Returns(userId);
        _harness.VehicleRepository.GetByIdAsync(vehicle.Id, Arg.Any<CancellationToken>()).Returns(vehicle);

        var result = await _harness.Sender.Send(new DeleteVehicleCommand(vehicle.Id));

        result.IsSuccess.Should().BeTrue();
        _harness.VehicleRepository.Received(1).Remove(vehicle);
    }

    [Fact]
    public async Task Handle_WhenVehicleBelongsToAnotherUser_ShouldReturnNotFoundAndNotRemove()
    {
        var vehicle = CreateExistingVehicle(Guid.NewGuid());

        _harness.UserContext.UserId.Returns(Guid.NewGuid());
        _harness.VehicleRepository.GetByIdAsync(vehicle.Id, Arg.Any<CancellationToken>()).Returns(vehicle);

        var result = await _harness.Sender.Send(new DeleteVehicleCommand(vehicle.Id));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(VehicleErrors.NotFound);
        _harness.VehicleRepository.DidNotReceive().Remove(vehicle);
    }
}
