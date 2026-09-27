using Dapper;
using Finance.Application.Abstractions.Data;
using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Abstracts;

namespace Finance.Application.Payments.SearchPayments;

internal sealed class SearchPaymentsQueryHandler : IQueryHandler<SearchPaymentsQuery, IReadOnlyList<PaymentResponse>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public SearchPaymentsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<IReadOnlyList<PaymentResponse>>> Handle(SearchPaymentsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        const string sql = """
            SELECT
                p.*
            FROM payments AS p
            """;

        var payments = await connection.QueryAsync<PaymentResponse>(sql);

        return payments.ToList();
    }
}
