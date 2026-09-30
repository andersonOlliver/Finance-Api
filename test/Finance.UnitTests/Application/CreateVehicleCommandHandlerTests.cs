using Finance.Application.Exceptions;
using Finance.Application.Vehicles.CreateVehicle;
using Finance.Domain.Vehicles;
using FluentAssertions;
using NSubstitute;

namespace Finance.UnitTests.Application;

public class CreateVehicleCommandHandlerTests
{
    private readonly ApplicationTestHarness _harness = new();

    [Fact]
    public async Task Handle_WithValidCommand_ShouldAddVehicleOwnedByCurrentUser()
    {
        var userId = Guid.NewGuid();
        _harness.UserContext.UserId.Returns(userId);

        var command = new CreateVehicleCommand("Meu Civic", VehicleType.Car, "Honda", "Civic", "Prata", 2020, "ABC1D23", 15000m);

        var result = await _harness.Sender.Send(command);

        result.IsSuccess.Should().BeTrue();

        _harness.VehicleRepository.Received(1).Add(Arg.Is<Vehicle>(v =>
            v.Id == result.Value &&
            v.Nickname.Value == "Meu Civic" &&
            v.Mileage == 15000m &&
            v.UserId == userId));

        await _harness.UnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithEmptyNickname_ShouldThrowValidationException()
    {
        var command = new CreateVehicleCommand(string.Empty, VehicleType.Car, "Honda", "Civic", "Prata", 2020, "ABC1D23", 15000m);

        var act = () => _harness.Sender.Send(command);

        await act.Should().ThrowAsync<ValidationException>();
        _harness.VehicleRepository.DidNotReceive().Add(Arg.Any<Vehicle>());
    }

    [Fact]
    public async Task Handle_WithNegativeInitialMileage_ShouldThrowValidationException()
    {
        var command = new CreateVehicleCommand("Meu Civic", VehicleType.Car, "Honda", "Civic", "Prata", 2020, "ABC1D23", -1m);

        var act = () => _harness.Sender.Send(command);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_WithInvalidYear_ShouldThrowValidationException()
    {
        var command = new CreateVehicleCommand("Meu Civic", VehicleType.Car, "Honda", "Civic", "Prata", 1800, "ABC1D23", 15000m);

        var act = () => _harness.Sender.Send(command);

        await act.Should().ThrowAsync<ValidationException>();
    }
}
