using Dapper;
using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Data;
using Finance.Application.Abstractions.Messaging;
using Finance.Application.Categories.SearchCategories;
using Finance.Domain.Abstracts;
using Finance.Domain.Categories;

namespace Finance.Application.Categories.GetCategoryById;

internal sealed class GetCategoryByIdQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory,
    IUserContext userContext) : IQueryHandler<GetCategoryByIdQuery, CategoryResponse>
{
    public async Task<Result<CategoryResponse>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
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
            WHERE c.id = @Id AND (c.user_id IS NULL OR c.user_id = @UserId)
            """;

        var category = await connection.QuerySingleOrDefaultAsync<CategoryResponse>(
            sql,
            new { request.Id, userContext.UserId });

        return category is null
            ? Result.Failure<CategoryResponse>(CategoryErrors.NotFound)
            : category;
    }
}
