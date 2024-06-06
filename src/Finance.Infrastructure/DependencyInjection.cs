using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Clock;
using Finance.Domain.Abstracts;
using Finance.Domain.Users;
using Finance.Infrastructure.Authentication;
using Finance.Infrastructure.Clock;
using Finance.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Finance.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddTransient<IAuthenticationService, AuthenticationService>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddTransient<IDateTimeProvider, DateTimeProvider>()
            .AddPersistence(configuration);
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database") ?? throw new ArgumentNullException(nameof(configuration));
        services.AddDbContext<ApplicationDbContext>(
            options => options.UseNpgsql(connectionString)
                            .UseSnakeCaseNamingConvention()
            );

        //services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        //services.AddSingleton<ISqlConnectionFactory>(_ => new SqlConnectionFactory(connectionString));

        //SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());

        return services;
    }
}
