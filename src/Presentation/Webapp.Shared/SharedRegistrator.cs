using Microsoft.Extensions.DependencyInjection;
using Webapp.Shared.Services;

namespace Webapp.Shared;

public static class SharedRegistrator
{
    public static void AddSharedServices(this IServiceCollection services)
    {
        services.AddScoped<ResponseHandler>();
        services.AddHttpClient("ApiClient", client =>
        {
            client.BaseAddress = new Uri("https://localhost:7210");
        });

        services.AddScoped<CategoryService>();
        services.AddScoped<ProductAttributeService>();
        services.AddScoped<ProductService>();
    }
}
