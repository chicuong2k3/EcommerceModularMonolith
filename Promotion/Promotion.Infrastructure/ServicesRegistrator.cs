using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Promotion.Infrastructure.Persistence.Repositories;
using Promotion.Infrastructure.Services;

namespace Promotion.Infrastructure;

public static class ServicesRegistrator
{
    public static void RegisterPromotionServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICouponRepository, CouponRepository>();
        services.AddScoped<IConditionRepository, ConditionRepository>();

        services.AddScoped<IDiscountService, DiscountService>();


    }
}
