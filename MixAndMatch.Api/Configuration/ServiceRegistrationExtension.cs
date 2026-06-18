using FluentValidation;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Application.UseCases.Categoria.Commands;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Api.Configuration;

public static class ServiceRegistrationExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Registro de servicios de infraestructura
        services.AddInfrastructureServices(configuration);

        // Registro de MediatR + behavior de validación
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(CreateCategoriaCommand).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
        });

        // Validadores de FluentValidation (mismo assembly que los use cases)
        services.AddValidatorsFromAssembly(typeof(CreateCategoriaCommand).Assembly);

        // Un mensaje por campo (corta en el primer error de cada RuleFor),
        // pero reporta todos los campos fallidos a la vez.
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

        // Controllers y Swagger
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
}
