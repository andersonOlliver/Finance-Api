using Dapper;
using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Data;
using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Abstracts;

namespace Finance.Application.Categories.SearchCategories;

internal sealed class SearchCategoriesQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory,
    IUserContext userContext) : IQueryHandler<SearchCategoryQuery, IReadOnlyList<CategoryResponse>>
{
    public async Task<Result<IReadOnlyList<CategoryResponse>>> Handle(SearchCategoryQuery request, CancellationToken cancellationToken)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        const string sql = """
            SELECT
                c.id AS Id,
                c.name AS Name,
                c.type AS Type,
                c.color AS Color,
                c.icon AS Icon,
                c.user_id AS UserId
            FROM categories AS c
            WHERE c.user_id IS NULL OR c.user_id = @UserId
            """;

        var categories = await connection.QueryAsync<CategoryResponse>(sql, new { userContext.UserId });

        return categories.ToList();
    }
}
