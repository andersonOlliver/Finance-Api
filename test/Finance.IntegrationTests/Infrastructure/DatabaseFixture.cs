using Finance.Application;
using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Clock;
using Finance.Application.Abstractions.Data;
using Finance.Domain.Abstracts;
using Finance.Domain.Transactions;
using Finance.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace Finance.IntegrationTests.Infrastructure;

public sealed class DatabaseFixture : IAsyncLifetime
{
    private PostgreSqlContainer _container = null!;

    internal IServiceProvider Services { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        _container = new PostgreSqlBuilder("postgres:17")
            .WithDatabase("finance_tests")
            .WithUsername("olliver")
            .WithPassword("q1w2e3r4")
            .Build();

        await _container.StartAsync();

        var connectionString = _container.GetConnectionString();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplication();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<ITransactionRepository, TestTransactionRepository>();
        services.AddSingleton<ISqlConnectionFactory>(new TestSqlConnectionFactory(connectionString));
        services.AddSingleton<TestUserContextAccessor>();
        services.AddScoped<IUserContext, TestUserContext>();
        services.AddSingleton<IDateTimeProvider, TestDateTimeProvider>();

        Services = services.BuildServiceProvider();

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}

[CollectionDefinition("Database")]
public sealed class DatabaseCollection : ICollectionFixture<DatabaseFixture>;
