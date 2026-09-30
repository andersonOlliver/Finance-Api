using Finance.Application.Vehicles.UpdateVehicle;
using Finance.Domain.Shared;
using Finance.Domain.Vehicles;
using FluentAssertions;
using NSubstitute;

namespace Finance.UnitTests.Application;

public class UpdateVehicleCommandHandlerTests
{
    private readonly ApplicationTestHarness _harness = new();

    private static Vehicle CreateExistingVehicle(Guid userId) =>
        Vehicle.Create(Guid.NewGuid(), new Name("Meu Civic"), VehicleType.Car, "Honda", "Civic", "Prata", 2020, "ABC1D23", 15000m, userId, DateTime.UtcNow);

    [Fact]
    public async Task Handle_WithOwnedVehicle_ShouldUpdateItAndSucceed()
    {
        var userId = Guid.NewGuid();
        var vehicle = CreateExistingVehicle(userId);

        _harness.UserContext.UserId.Returns(userId);
        _harness.VehicleRepository.GetByIdAsync(vehicle.Id, Arg.Any<CancellationToken>()).Returns(vehicle);

        var command = new UpdateVehicleCommand(vehicle.Id, "Civic da família", VehicleType.Car, "Honda", "Civic EXL", "Preto", 2021, "XYZ9Z99");

        var result = await _harness.Sender.Send(command);

        result.IsSuccess.Should().BeTrue();
        vehicle.Nickname.Value.Should().Be("Civic da família");
        vehicle.Model.Should().Be("Civic EXL");
        vehicle.Mileage.Should().Be(15000m);

        await _harness.UnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenVehicleBelongsToAnotherUser_ShouldReturnNotFound()
    {
        var vehicle = CreateExistingVehicle(Guid.NewGuid());

        _harness.UserContext.UserId.Returns(Guid.NewGuid());
        _harness.VehicleRepository.GetByIdAsync(vehicle.Id, Arg.Any<CancellationToken>()).Returns(vehicle);

        var command = new UpdateVehicleCommand(vehicle.Id, "Hackeado", VehicleType.Car, "Honda", "Civic", "Prata", 2020, "ABC1D23");

        var result = await _harness.Sender.Send(command);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(VehicleErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenVehicleDoesNotExist_ShouldReturnNotFound()
    {
        _harness.UserContext.UserId.Returns(Guid.NewGuid());
        _harness.VehicleRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Vehicle?)null);

        var command = new UpdateVehicleCommand(Guid.NewGuid(), "Meu Civic", VehicleType.Car, "Honda", "Civic", "Prata", 2020, "ABC1D23");

        var result = await _harness.Sender.Send(command);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(VehicleErrors.NotFound);
    }
}
