using Dapper;
using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Data;
using Finance.Application.Abstractions.Messaging;
using Finance.Application.CreditCards.SearchCreditCards;
using Finance.Domain.Abstracts;
using Finance.Domain.CreditCards;

namespace Finance.Application.CreditCards.GetCreditCardById;

internal sealed class GetCreditCardByIdQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory,
    IUserContext userContext) : IQueryHandler<GetCreditCardByIdQuery, CreditCardResponse>
{
    public async Task<Result<CreditCardResponse>> Handle(GetCreditCardByIdQuery request, CancellationToken cancellationToken)
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
            WHERE c.id = @Id AND c.user_id = @UserId
            """;

        var creditCard = await connection.QuerySingleOrDefaultAsync<CreditCardResponse>(
            sql,
            new { request.Id, userContext.UserId });

        return creditCard is null
            ? Result.Failure<CreditCardResponse>(CreditCardErrors.NotFound)
            : creditCard;
    }
}
