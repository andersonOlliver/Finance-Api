using Finance.Application.Transactions.GetTransactionById;
using Finance.Application.Vehicles.CreateVehicle;
using Finance.Application.Vehicles.CreateVehicleRefuel;
using Finance.Application.Vehicles.GetVehicleById;
using Finance.Application.Vehicles.SearchVehicleRefuels;
using Finance.Domain.Categories;
using Finance.Domain.Shared;
using Finance.Domain.Users;
using Finance.Domain.Vehicles;
using Finance.Infrastructure;
using Finance.IntegrationTests.Infrastructure;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Finance.IntegrationTests.Vehicles;

[Collection("Database")]
public class VehicleRefuelFlowTests(DatabaseFixture fixture)
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

    private async Task<Guid> SeedCategoryAsync()
    {
        using var scope = fixture.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var category = Category.Create(Guid.NewGuid(), new Name("Combustível"), CategoryType.Expense, Color.Orange, Icon.Transfer, DateTime.UtcNow);
        dbContext.Set<Category>().Add(category);
        await dbContext.SaveChangesWithoutEventsAsync();

        return category.Id;
    }

    private void SetCurrentUser(Guid userId)
    {
        fixture.Services.GetRequiredService<TestUserContextAccessor>().UserId = userId;
    }

    [Fact]
    public async Task Should_create_transaction_update_mileage_and_compute_consumption_across_two_refuels()
    {
        var userId = await SeedUserAsync();
        SetCurrentUser(userId);
        var categoryId = await SeedCategoryAsync();

        Guid vehicleId;
        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();
            var createVehicleResult = await sender.Send(new CreateVehicleCommand(
                "Meu Civic", VehicleType.Car, "Honda", "Civic", "Prata", 2020, "ABC1D23", 15000m));

            createVehicleResult.IsSuccess.Should().BeTrue();
            vehicleId = createVehicleResult.Value;
        }

        Guid firstTransactionId;
        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();

            var firstRefuelResult = await sender.Send(new CreateVehicleRefuelCommand(
                vehicleId, 40m, 5m, "USD", 15400m, categoryId, null, "Posto Ipiranga", DateTime.UtcNow));

            firstRefuelResult.IsSuccess.Should().BeTrue();
            firstRefuelResult.Value.Amount.Should().Be(200m);
            firstRefuelResult.Value.DistanceSinceLastRefuel.Should().Be(400m);
            firstRefuelResult.Value.ConsumptionSinceLastRefuel.Should().Be(10m);

            firstTransactionId = firstRefuelResult.Value.TransactionId;
        }

        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();

            var vehicleResult = await sender.Send(new GetVehicleByIdQuery(vehicleId));
            vehicleResult.Value.Mileage.Should().Be(15400m);

            var transactionResult = await sender.Send(new GetTransactionByIdQuery(firstTransactionId));
            transactionResult.IsSuccess.Should().BeTrue();
            transactionResult.Value.VehicleId.Should().Be(vehicleId);
            transactionResult.Value.Amount.Should().Be(200m);
            transactionResult.Value.CategoryId.Should().Be(categoryId);
        }

        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();

            var secondRefuelResult = await sender.Send(new CreateVehicleRefuelCommand(
                vehicleId, 40m, 5.5m, "USD", 15800m, categoryId, null, null, DateTime.UtcNow));

            secondRefuelResult.IsSuccess.Should().BeTrue();
            secondRefuelResult.Value.DistanceSinceLastRefuel.Should().Be(400m);
            secondRefuelResult.Value.ConsumptionSinceLastRefuel.Should().Be(10m);
        }

        using (var scope = fixture.Services.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();

            var vehicleResult = await sender.Send(new GetVehicleByIdQuery(vehicleId));
            vehicleResult.Value.Mileage.Should().Be(15800m);

            var refuelsResult = await sender.Send(new SearchVehicleRefuelsQuery(vehicleId));
            refuelsResult.Value.Should().HaveCount(2);
            refuelsResult.Value.Should().OnlyContain(r => r.VehicleId == vehicleId);
        }
    }

    [Fact]
    public async Task Should_fail_when_mileage_is_not_greater_than_the_vehicles_current_mileage()
    {
        var userId = await SeedUserAsync();
        SetCurrentUser(userId);
        var categoryId = await SeedCategoryAsync();

        using var scope = fixture.Services.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        var createVehicleResult = await sender.Send(new CreateVehicleCommand(
            "Meu Civic", VehicleType.Car, "Honda", "Civic", "Prata", 2020, "ABC1D23", 15000m));
        var vehicleId = createVehicleResult.Value;

        var refuelResult = await sender.Send(new CreateVehicleRefuelCommand(
            vehicleId, 40m, 5m, "USD", 15000m, categoryId, null, null, DateTime.UtcNow));

        refuelResult.IsFailure.Should().BeTrue();
        refuelResult.Error.Should().Be(VehicleErrors.MileageMustBeGreaterThanCurrent);
    }
}
