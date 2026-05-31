using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MixAndMatch.Domain.Ports;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Domain.Ports.IServices;
using MixAndMatch.Infrastructure.Adapters;
using MixAndMatch.Infrastructure.Adapters.Services;
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
        services.AddScoped<IReseniaRepository, ReseniaRepository>();
        services.AddScoped<IPasswordService, PasswordService>();

        // Middlewares
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddApiRateLimiter();

        return services;
    }
}
