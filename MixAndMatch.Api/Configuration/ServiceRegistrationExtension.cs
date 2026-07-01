using FluentValidation;
using MediatR;
using Microsoft.OpenApi.Models;
using MixAndMatch.Application.Common;
using MixAndMatch.Application.Jobs;
using MixAndMatch.Application.Services;
using MixAndMatch.Application.UseCases.Categoria.Commands;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Api.Configuration;

public static class ServiceRegistrationExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Registro de servicios de infraestructura
        services.AddInfrastructureServices(configuration);

        // Application services (lógica compartida entre use cases)
        services.AddScoped<IConfirmacionPagoService, ConfirmacionPagoService>();

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

        // Hangfire jobs (DI para que Hangfire los resuelva al ejecutar)
        services.AddScoped<SincronizarEstadosDescuentosJob>();
        services.AddScoped<ExpirarCodigosPorUsoJob>();
        services.AddScoped<LimpiarCarritosAbandonadosJob>();

        // CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader());
        });

        // Controllers y Swagger
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name         = "Authorization",
                Type         = SecuritySchemeType.Http,
                Scheme       = "Bearer",
                BearerFormat = "JWT",
                In           = ParameterLocation.Header,
                Description  = "Ingresa el token JWT. Ejemplo: Bearer {token}"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id   = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}
