using Dapper;
using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Data;
using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Abstracts;

namespace Finance.Application.Transactions.SearchTransactions;

internal sealed class SearchTransactionsQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory,
    IUserContext userContext) : IQueryHandler<SearchTransactionsQuery, IReadOnlyList<TransactionResponse>>
{
    public async Task<Result<IReadOnlyList<TransactionResponse>>> Handle(SearchTransactionsQuery request, CancellationToken cancellationToken)
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
            WHERE t.user_id = @UserId
                AND (@CategoryId IS NULL OR t.category_id = @CategoryId)
                AND (@PaymentId IS NULL OR t.payment_id = @PaymentId)
                AND (@From IS NULL OR t.released_on_utc >= @From)
                AND (@To IS NULL OR t.released_on_utc <= @To)
            ORDER BY t.released_on_utc DESC
            """;

        var transactions = await connection.QueryAsync<TransactionResponse>(
            sql,
            new
            {
                userContext.UserId,
                request.CategoryId,
                request.PaymentId,
                request.From,
                request.To
            });

        return transactions.ToList();
    }
}
