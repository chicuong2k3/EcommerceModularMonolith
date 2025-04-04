using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Infrastructure.Persistence.Repositories;
using Ordering.Infrastructure.Services;

namespace Ordering.Infrastructure;

public static class ServicesRegistrator
{
    public static void RegisterOrderingServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICartRepository, CartRepository>();
        services.Decorate<ICartRepository, CachedCartRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        services.AddScoped<IProductService, ProuductService>();
        services.AddScoped<ICouponService, CouponClient>();

        services.AddHttpClient("PromotionService", client =>
        {
            var uri = configuration.GetConnectionString("PromotionService")
                ?? throw new ArgumentNullException("PromotionService Service Uri is not configured.");
            client.BaseAddress = new Uri(uri);
        });

    }
}
