using Common.Domain.PaymentStrategy;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCommonInfrastructure(this IServiceCollection services)
    {
        // Register payment strategies
        services.AddScoped<IPaymentStrategy, CODPaymentStrategy>();
        services.AddScoped<IPaymentStrategy, OnlinePaymentStrategy>();
        services.AddScoped<IPaymentStrategyFactory, PaymentStrategyFactory>();

        return services;
    }
}
