using Dapper;
using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Data;
using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Abstracts;

namespace Finance.Application.CreditCards.SearchCreditCards;

internal sealed class SearchCreditCardsQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory,
    IUserContext userContext) : IQueryHandler<SearchCreditCardsQuery, IReadOnlyList<CreditCardResponse>>
{
    public async Task<Result<IReadOnlyList<CreditCardResponse>>> Handle(SearchCreditCardsQuery request, CancellationToken cancellationToken)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        const string sql = """
            SELECT
                c.id AS Id,
                c.nickname AS Nickname,
                c.brand AS Brand,
                c.due_day AS DueDay,
                c.created_on_utc AS CreatedOnUtc,
                c.updated_on_utc AS UpdatedOnUtc
            FROM credit_cards AS c
            WHERE c.user_id = @UserId
            ORDER BY c.created_on_utc
            """;

        var creditCards = await connection.QueryAsync<CreditCardResponse>(sql, new { userContext.UserId });

        return creditCards.ToList();
    }
}
