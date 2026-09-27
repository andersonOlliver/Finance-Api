using Dapper;
using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Data;
using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Abstracts;

namespace Finance.Application.Payments.SearchPayments;

internal sealed class SearchPaymentsQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory,
    IUserContext userContext) : IQueryHandler<SearchPaymentsQuery, IReadOnlyList<PaymentResponse>>
{
    public async Task<Result<IReadOnlyList<PaymentResponse>>> Handle(SearchPaymentsQuery request, CancellationToken cancellationToken)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        const string sql = """
            SELECT
                p.id AS Id,
                p.name AS Name,
                p.type AS Type,
                p.user_id AS UserId
            FROM payments AS p
            WHERE p.user_id IS NULL OR p.user_id = @UserId
            """;

        var payments = await connection.QueryAsync<PaymentResponse>(sql, new { userContext.UserId });

        return payments.ToList();
    }
}
