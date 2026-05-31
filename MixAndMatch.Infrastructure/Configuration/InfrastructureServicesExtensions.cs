using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Infrastructure.Adapters;
using MixAndMatch.Infrastructure.Middlewares;

namespace MixAndMatch.Infrastructure.Configuration;

public static class InfrastructureServicesExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database Connection
        services.AddDbContext<MixAndMatchDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseNpgsql(connectionString);
        });

        // Services Register
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Middlewares
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddApiRateLimiter();

        return services;
    }
}
