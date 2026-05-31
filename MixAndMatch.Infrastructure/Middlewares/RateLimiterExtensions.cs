using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.RateLimiting;

namespace MixAndMatch.Infrastructure.Middlewares;

public static class RateLimiterExtensions
{
    public static IServiceCollection AddApiRateLimiter(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = 429;

            options.AddFixedWindowLimiter("fixed", limiter =>
            {
                limiter.PermitLimit         = 100;
                limiter.Window              = TimeSpan.FromMinutes(1);
                limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiter.QueueLimit          = 10;
            });
        });

        return services;
    }
}
