using Dapper;
using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Clock;
using Finance.Application.Abstractions.Data;
using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Abstracts;

namespace Finance.Application.Installments.SearchInstallmentPurchases;

internal sealed class SearchInstallmentPurchasesQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider) : IQueryHandler<SearchInstallmentPurchasesQuery, IReadOnlyList<InstallmentPurchaseStatusResponse>>
{
    public async Task<Result<IReadOnlyList<InstallmentPurchaseStatusResponse>>> Handle(SearchInstallmentPurchasesQuery request, CancellationToken cancellationToken)
    {
        var referenceMonth = request.Month ?? dateTimeProvider.UtcNow;
        var monthStart = new DateTime(referenceMonth.Year, referenceMonth.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var monthEnd = monthStart.AddMonths(1);

        using var connection = sqlConnectionFactory.CreateConnection();

        const string sql = """
            SELECT
                ip.id AS InstallmentPurchaseId,
                t.id AS TransactionId,
                ip.title AS Title,
                ip.total_amount_amount AS TotalAmount,
                ip.total_amount_currency AS Currency,
                ip.installment_count AS InstallmentCount,
                t.installment_number AS CurrentInstallmentNumber,
                t.value_amount AS CurrentInstallmentAmount,
                t.released_on_utc AS ReleasedOnUtc,
                ip.category_id AS CategoryId,
                c.name AS CategoryName,
                ip.credit_card_id AS CreditCardId,
                cc.nickname AS CreditCardNickname
            FROM transactions AS t
            INNER JOIN installment_purchases AS ip ON ip.id = t.installment_purchase_id
            INNER JOIN categories AS c ON c.id = ip.category_id
            INNER JOIN credit_cards AS cc ON cc.id = ip.credit_card_id
            WHERE t.user_id = @UserId
                AND t.released_on_utc >= @MonthStart
                AND t.released_on_utc < @MonthEnd
                AND (@CreditCardId::uuid IS NULL OR ip.credit_card_id = @CreditCardId::uuid)
            ORDER BY ip.created_on_utc, t.installment_number
            """;

        var results = await connection.QueryAsync<InstallmentPurchaseStatusResponse>(
            sql,
            new
            {
                userContext.UserId,
                MonthStart = monthStart,
                MonthEnd = monthEnd,
                request.CreditCardId
            });

        return results.ToList();
    }
}
