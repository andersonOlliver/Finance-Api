using Finance.Domain.Shared;
using Finance.Domain.Vehicles;
using FluentAssertions;

namespace Finance.UnitTests.Domain;

public class VehicleTests
{
    [Fact]
    public void Create_ShouldSetAllProperties()
    {
        var id = Guid.NewGuid();
        var nickname = new Name("Meu Civic");
        var userId = Guid.NewGuid();
        var createdOnUtc = DateTime.UtcNow;

        var vehicle = Vehicle.Create(id, nickname, VehicleType.Car, "Honda", "Civic", "Prata", 2020, "ABC1D23", 15000m, userId, createdOnUtc);

        vehicle.Id.Should().Be(id);
        vehicle.Nickname.Should().Be(nickname);
        vehicle.Type.Should().Be(VehicleType.Car);
        vehicle.Brand.Should().Be("Honda");
        vehicle.Model.Should().Be("Civic");
        vehicle.Color.Should().Be("Prata");
        vehicle.Year.Should().Be(2020);
        vehicle.LicensePlate.Should().Be("ABC1D23");
        vehicle.Mileage.Should().Be(15000m);
        vehicle.UserId.Should().Be(userId);
        vehicle.CreatedOnUtc.Should().Be(createdOnUtc);
        vehicle.UpdatedOnUtc.Should().BeNull();
    }

    [Fact]
    public void Update_ShouldReplaceDescriptiveFieldsButNotMileage()
    {
        var vehicle = Vehicle.Create(
            Guid.NewGuid(), new Name("Meu Civic"), VehicleType.Car, "Honda", "Civic", "Prata", 2020, "ABC1D23", 15000m, Guid.NewGuid(), DateTime.UtcNow);

        var updatedOnUtc = DateTime.UtcNow.AddMinutes(5);
        vehicle.Update(new Name("Civic da família"), VehicleType.Car, "Honda", "Civic EXL", "Preto", 2021, "XYZ9Z99", updatedOnUtc);

        vehicle.Nickname.Value.Should().Be("Civic da família");
        vehicle.Model.Should().Be("Civic EXL");
        vehicle.Color.Should().Be("Preto");
        vehicle.Year.Should().Be(2021);
        vehicle.LicensePlate.Should().Be("XYZ9Z99");
        vehicle.UpdatedOnUtc.Should().Be(updatedOnUtc);
        vehicle.Mileage.Should().Be(15000m);
    }

    [Fact]
    public void UpdateMileage_ShouldReplaceMileageAndSetUpdatedOnUtc()
    {
        var vehicle = Vehicle.Create(
            Guid.NewGuid(), new Name("Meu Civic"), VehicleType.Car, "Honda", "Civic", "Prata", 2020, "ABC1D23", 15000m, Guid.NewGuid(), DateTime.UtcNow);

        var updatedOnUtc = DateTime.UtcNow.AddDays(1);
        vehicle.UpdateMileage(15450m, updatedOnUtc);

        vehicle.Mileage.Should().Be(15450m);
        vehicle.UpdatedOnUtc.Should().Be(updatedOnUtc);
    }
}
