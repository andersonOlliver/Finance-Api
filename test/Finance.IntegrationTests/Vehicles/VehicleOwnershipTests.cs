using Finance.Application.Vehicles.CreateVehicle;
using Finance.Application.Vehicles.DeleteVehicle;
using Finance.Application.Vehicles.SearchVehicles;
using Finance.Application.Vehicles.UpdateVehicle;
using Finance.Domain.Users;
using Finance.Domain.Vehicles;
using Finance.Infrastructure;
using Finance.IntegrationTests.Infrastructure;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Finance.IntegrationTests.Vehicles;

[Collection("Database")]
public class VehicleOwnershipTests(DatabaseFixture fixture)
{
    private async Task<Guid> SeedUserAsync()
    {
        using var scope = fixture.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var user = User.Create(
            new FirstName("Test"),
            new LastName("User"),
            new Email($"user-{Guid.NewGuid():N}@test.local"),
            DateTime.UtcNow);
        user.SetIdentityId(string.Empty);

        dbContext.Set<User>().Add(user);
        await dbContext.SaveChangesWithoutEventsAsync();

        return user.Id;
    }

    private void SetCurrentUser(Guid userId)
    {
        fixture.Services.GetRequiredService<TestUserContextAccessor>().UserId = userId;
    }

    [Fact]
    public async Task SearchVehicles_ShouldReturnOnlyTheCurrentUsersVehicles()
    {
        var userAId = await SeedUserAsync();
        var userBId = await SeedUserAsync();

        SetCurrentUser(userAId);
        Guid vehicleAId;
        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();
            vehicleAId = (await sender.Send(new CreateVehicleCommand("Civic da Usuária A", VehicleType.Car, "Honda", "Civic", "Prata", 2020, "AAA1A11", 10000m))).Value;
        }

        SetCurrentUser(userBId);
        Guid vehicleBId;
        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();
            vehicleBId = (await sender.Send(new CreateVehicleCommand("Moto do Usuário B", VehicleType.Motorcycle, "Honda", "CG 160", "Vermelho", 2019, "BBB2B22", 8000m))).Value;
        }

        SetCurrentUser(userAId);
        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();
            var result = await sender.Send(new SearchVehiclesQuery());

            result.Value.Should().Contain(v => v.Id == vehicleAId);
            result.Value.Should().NotContain(v => v.Id == vehicleBId);
        }
    }

    [Fact]
    public async Task UpdateAndDeleteVehicle_ForAnotherUsersVehicle_ShouldReturnNotFound()
    {
        var ownerId = await SeedUserAsync();
        var otherUserId = await SeedUserAsync();

        SetCurrentUser(ownerId);
        Guid vehicleId;
        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();
            vehicleId = (await sender.Send(new CreateVehicleCommand("Meu Civic", VehicleType.Car, "Honda", "Civic", "Prata", 2020, "AAA1A11", 10000m))).Value;
        }

        SetCurrentUser(otherUserId);
        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();

            var updateResult = await sender.Send(new UpdateVehicleCommand(vehicleId, "Hackeado", VehicleType.Car, "Honda", "Civic", "Prata", 2020, "AAA1A11"));
            updateResult.IsFailure.Should().BeTrue();
            updateResult.Error.Should().Be(VehicleErrors.NotFound);

            var deleteResult = await sender.Send(new DeleteVehicleCommand(vehicleId));
            deleteResult.IsFailure.Should().BeTrue();
            deleteResult.Error.Should().Be(VehicleErrors.NotFound);
        }
    }
}
