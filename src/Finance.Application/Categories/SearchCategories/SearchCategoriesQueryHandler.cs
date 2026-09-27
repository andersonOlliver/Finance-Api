using Dapper;
using Finance.Application.Abstractions.Data;
using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Abstracts;

namespace Finance.Application.Categories.SearchCategories;

internal sealed class SearchCategoriesQueryHandler : IQueryHandler<SearchCategoryQuery, IReadOnlyList<CategoryResponse>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public SearchCategoriesQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<IReadOnlyList<CategoryResponse>>> Handle(SearchCategoryQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        const string sql = """
            SELECT 
                c.*
            FROM categories AS c
            """;

        var categories = await connection.QueryAsync<CategoryResponse>(sql);

        return categories.ToList();
    }
}
