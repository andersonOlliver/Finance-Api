using Dapper;
using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Data;
using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Abstracts;

namespace Finance.Application.Vehicles.SearchVehicleRefuels;

internal sealed class SearchVehicleRefuelsQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory,
    IUserContext userContext) : IQueryHandler<SearchVehicleRefuelsQuery, IReadOnlyList<VehicleRefuelListItemResponse>>
{
    public async Task<Result<IReadOnlyList<VehicleRefuelListItemResponse>>> Handle(SearchVehicleRefuelsQuery request, CancellationToken cancellationToken)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        const string sql = """
            SELECT
                r.id AS Id,
                r.vehicle_id AS VehicleId,
                r.transaction_id AS TransactionId,
                r.liters AS Liters,
                r.price_per_liter AS PricePerLiter,
                t.value_amount AS Amount,
                r.mileage AS Mileage,
                r.consumption_since_last_refuel AS ConsumptionSinceLastRefuel,
                r.refueled_on_utc AS RefueledOnUtc
            FROM vehicle_refuels AS r
            INNER JOIN vehicles AS v ON v.id = r.vehicle_id
            INNER JOIN transactions AS t ON t.id = r.transaction_id
            WHERE r.vehicle_id = @VehicleId AND v.user_id = @UserId
            ORDER BY r.refueled_on_utc DESC
            """;

        var refuels = await connection.QueryAsync<VehicleRefuelListItemResponse>(
            sql,
            new { request.VehicleId, userContext.UserId });

        return refuels.ToList();
    }
}
