using MediatR;
using MixAndMatch.Application.UseCases.Categoria.Commands;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Api.Configuration;

public static class ServiceRegistrationExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructureServices(configuration);
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreateCategoriaCommand).Assembly));

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
}
