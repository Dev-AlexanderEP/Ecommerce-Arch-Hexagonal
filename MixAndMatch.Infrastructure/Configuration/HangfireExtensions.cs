using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MixAndMatch.Infrastructure.Configuration;

public static class HangfireExtensions
{
    public static IServiceCollection AddHangfireWithPostgres(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")!;

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(opt => opt.UseNpgsqlConnection(connectionString)));

        services.AddHangfireServer(opt =>
        {
            opt.WorkerCount = 2;
            opt.ServerName = "MixAndMatch-Worker";
        });

        return services;
    }
}
