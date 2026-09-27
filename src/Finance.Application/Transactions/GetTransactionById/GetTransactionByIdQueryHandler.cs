using Dapper;
using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Data;
using Finance.Application.Abstractions.Messaging;
using Finance.Application.Transactions.SearchTransactions;
using Finance.Domain.Abstracts;
using Finance.Domain.Transactions;

namespace Finance.Application.Transactions.GetTransactionById;

internal sealed class GetTransactionByIdQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory,
    IUserContext userContext) : IQueryHandler<GetTransactionByIdQuery, TransactionResponse>
{
    public async Task<Result<TransactionResponse>> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        const string sql = """
            SELECT
                t.id AS Id,
                t.title AS Title,
                t.value_amount AS Amount,
                t.value_currency AS Currency,
                t.description AS Description,
                t.category_id AS CategoryId,
                c.name AS CategoryName,
                c.type AS CategoryType,
                c.color AS CategoryColor,
                c.icon AS CategoryIcon,
                t.payment_id AS PaymentId,
                p.name AS PaymentName,
                p.type AS PaymentType,
                t.released_on_utc AS ReleasedOnUtc,
                t.created_on_utc AS CreatedOnUtc,
                t.updated_on_utc AS UpdatedOnUtc
            FROM transactions AS t
            INNER JOIN categories AS c ON c.id = t.category_id
            LEFT JOIN payments AS p ON p.id = t.payment_id
            WHERE t.id = @Id AND t.user_id = @UserId
            """;

        var transaction = await connection.QuerySingleOrDefaultAsync<TransactionResponse>(
            sql,
            new
            {
                request.Id,
                userContext.UserId
            });

        return transaction is null
            ? Result.Failure<TransactionResponse>(TransactionErrors.NotFound)
            : transaction;
    }
}
