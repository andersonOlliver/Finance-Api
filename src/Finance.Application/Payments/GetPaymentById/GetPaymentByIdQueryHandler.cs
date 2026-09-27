using Dapper;
using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Data;
using Finance.Application.Abstractions.Messaging;
using Finance.Application.Payments.SearchPayments;
using Finance.Domain.Abstracts;
using Finance.Domain.Payments;

namespace Finance.Application.Payments.GetPaymentById;

internal sealed class GetPaymentByIdQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory,
    IUserContext userContext) : IQueryHandler<GetPaymentByIdQuery, PaymentResponse>
{
    public async Task<Result<PaymentResponse>> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        const string sql = """
            SELECT
                p.id AS Id,
                p.name AS Name,
                p.type AS Type,
                p.user_id AS UserId
            FROM payments AS p
            WHERE p.id = @Id AND (p.user_id IS NULL OR p.user_id = @UserId)
            """;

        var payment = await connection.QuerySingleOrDefaultAsync<PaymentResponse>(
            sql,
            new { request.Id, userContext.UserId });

        return payment is null
            ? Result.Failure<PaymentResponse>(PaymentErrors.NotFound)
            : payment;
    }
}
