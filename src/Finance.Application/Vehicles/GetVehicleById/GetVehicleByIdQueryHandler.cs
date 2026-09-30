using Dapper;
using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Data;
using Finance.Application.Abstractions.Messaging;
using Finance.Application.Vehicles.SearchVehicles;
using Finance.Domain.Abstracts;
using Finance.Domain.Vehicles;

namespace Finance.Application.Vehicles.GetVehicleById;

internal sealed class GetVehicleByIdQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory,
    IUserContext userContext) : IQueryHandler<GetVehicleByIdQuery, VehicleResponse>
{
    public async Task<Result<VehicleResponse>> Handle(GetVehicleByIdQuery request, CancellationToken cancellationToken)
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
            WHERE v.id = @Id AND v.user_id = @UserId
            """;

        var vehicle = await connection.QuerySingleOrDefaultAsync<VehicleResponse>(
            sql,
            new { request.Id, userContext.UserId });

        return vehicle is null
            ? Result.Failure<VehicleResponse>(VehicleErrors.NotFound)
            : vehicle;
    }
}
