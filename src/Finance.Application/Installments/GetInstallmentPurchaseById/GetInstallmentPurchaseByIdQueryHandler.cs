using Dapper;
using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Data;
using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Abstracts;
using Finance.Domain.Installments;

namespace Finance.Application.Installments.GetInstallmentPurchaseById;

internal sealed class GetInstallmentPurchaseByIdQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory,
    IUserContext userContext) : IQueryHandler<GetInstallmentPurchaseByIdQuery, InstallmentPurchaseDetailResponse>
{
    public async Task<Result<InstallmentPurchaseDetailResponse>> Handle(GetInstallmentPurchaseByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        const string headerSql = """
            SELECT
                ip.id AS Id,
                ip.title AS Title,
                ip.total_amount_amount AS TotalAmount,
                ip.total_amount_currency AS Currency,
                ip.installment_count AS InstallmentCount,
                ip.category_id AS CategoryId,
                c.name AS CategoryName,
                ip.credit_card_id AS CreditCardId,
                cc.nickname AS CreditCardNickname,
                ip.first_installment_released_on_utc AS FirstInstallmentReleasedOnUtc,
                ip.created_on_utc AS CreatedOnUtc
            FROM installment_purchases AS ip
            INNER JOIN categories AS c ON c.id = ip.category_id
            INNER JOIN credit_cards AS cc ON cc.id = ip.credit_card_id
            WHERE ip.id = @Id AND ip.user_id = @UserId
            """;

        var installmentPurchase = await connection.QuerySingleOrDefaultAsync<InstallmentPurchaseDetailResponse>(
            headerSql,
            new { request.Id, userContext.UserId });

        if (installmentPurchase is null)
        {
            return Result.Failure<InstallmentPurchaseDetailResponse>(InstallmentPurchaseErrors.NotFound);
        }

        const string installmentsSql = """
            SELECT
                t.id AS TransactionId,
                t.installment_number AS InstallmentNumber,
                t.value_amount AS Amount,
                t.released_on_utc AS ReleasedOnUtc
            FROM transactions AS t
            WHERE t.installment_purchase_id = @Id
            ORDER BY t.installment_number
            """;

        var installments = await connection.QueryAsync<InstallmentLineResponse>(installmentsSql, new { request.Id });

        return new InstallmentPurchaseDetailResponse
        {
            Id = installmentPurchase.Id,
            Title = installmentPurchase.Title,
            TotalAmount = installmentPurchase.TotalAmount,
            Currency = installmentPurchase.Currency,
            InstallmentCount = installmentPurchase.InstallmentCount,
            CategoryId = installmentPurchase.CategoryId,
            CategoryName = installmentPurchase.CategoryName,
            CreditCardId = installmentPurchase.CreditCardId,
            CreditCardNickname = installmentPurchase.CreditCardNickname,
            FirstInstallmentReleasedOnUtc = installmentPurchase.FirstInstallmentReleasedOnUtc,
            CreatedOnUtc = installmentPurchase.CreatedOnUtc,
            Installments = installments.ToList()
        };
    }
}
