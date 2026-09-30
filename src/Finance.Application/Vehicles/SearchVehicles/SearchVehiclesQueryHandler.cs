using Dapper;
using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Data;
using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Abstracts;

namespace Finance.Application.Vehicles.SearchVehicles;

internal sealed class SearchVehiclesQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory,
    IUserContext userContext) : IQueryHandler<SearchVehiclesQuery, IReadOnlyList<VehicleResponse>>
{
    public async Task<Result<IReadOnlyList<VehicleResponse>>> Handle(SearchVehiclesQuery request, CancellationToken cancellationToken)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        const string sql = """
            SELECT
                v.id AS Id,
                v.nickname AS Nickname,
                v.type AS Type,
                v.brand AS Brand,
                v.model AS Model,
                v.color AS Color,
                v.year AS Year,
                v.license_plate AS LicensePlate,
                v.mileage AS Mileage,
                v.created_on_utc AS CreatedOnUtc,
                v.updated_on_utc AS UpdatedOnUtc
            FROM vehicles AS v
            WHERE v.user_id = @UserId
            ORDER BY v.created_on_utc
            """;

        var vehicles = await connection.QueryAsync<VehicleResponse>(sql, new { userContext.UserId });

        return vehicles.ToList();
    }
}
