using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Domain.Ports.IServices;
using MixAndMatch.Infrastructure.Adapters;
using MixAndMatch.Infrastructure.Adapters.Services;
using MixAndMatch.Infrastructure.Middlewares;
using StackExchange.Redis;

namespace MixAndMatch.Infrastructure.Configuration;

public static class InfrastructureServicesExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database Connection
        services.AddDbContext<MixAndMatchDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseNpgsql(connectionString, npgsql =>
            {
                var translator = new ExactNameTranslator();
                npgsql.MapEnum<RolUsuario>("rol_usuario", nameTranslator: translator);
                npgsql.MapEnum<TipoImagen>("tipo_imagen", nameTranslator: translator);
                npgsql.MapEnum<EstadoCarrito>("estado_carrito", nameTranslator: translator);
                npgsql.MapEnum<EstadoVenta>("estado_venta", nameTranslator: translator);
                npgsql.MapEnum<EstadoPago>("estado_pago", nameTranslator: translator);
                npgsql.MapEnum<EstadoEnvio>("estado_envio", nameTranslator: translator);
            });
        });

        // Services Register
        // Los repositorios especificos los expone el propio UnitOfWork (no se registran aparte).
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IJwtService, JwtService>();

        // Google OAuth
        services.Configure<GoogleSettings>(configuration.GetSection(GoogleSettings.SectionName));
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();

        // JWT Authentication
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
            ?? throw new InvalidOperationException("Falta la sección 'Jwt' en appsettings.json");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
                };
            });

        services.AddAuthorization();

        // Redis
        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));
        services.AddScoped<ICacheService, RedisCacheService>();

        // Notifications (SMTP Gmail)
        services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));
        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddScoped<IEmailTemplateService, EmailTemplateService>();

        // Middlewares
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddApiRateLimiter();

        // Hangfire
        services.AddHangfireWithPostgres(configuration);

        return services;
    }
}
