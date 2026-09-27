using System.Data;
using Finance.Application.Abstractions.Data;
using Npgsql;

namespace Finance.IntegrationTests.Infrastructure;

internal sealed class TestSqlConnectionFactory(string connectionString) : ISqlConnectionFactory
{
    public IDbConnection CreateConnection()
    {
        var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        return connection;
    }
}
