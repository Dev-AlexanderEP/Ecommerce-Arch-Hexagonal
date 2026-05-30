using MediatR;
using MixAndMatch.Application.UseCases.Categoria.Commands;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Api.Configuration;

public static class ServiceRegistrationExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Registro de servicios de infraestructura
        services.AddInfrastructureServices(configuration);

        // Registro de MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreateCategoriaCommand).Assembly));

        // Controllers y Swagger
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
}
