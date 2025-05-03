using Blazorise;
using Blazorise.Icons.FontAwesome;
using Blazorise.Tailwind;
using Microsoft.Extensions.DependencyInjection;
using Webapp.Shared.Services;

namespace Webapp.Shared;

public static class ShareConfig
{
    public static IServiceCollection AddShared(this IServiceCollection services)
    {
        services
            .AddBlazorise(options => { options.Immediate = true; })
            .AddTailwindProviders()
            .AddFontAwesomeIcons();

        services.AddScoped<ResponseHandler>();
        services.AddHttpClient("ApiClient", client =>
        {
            client.BaseAddress = new Uri("https://localhost:7210");
        });

        services.AddScoped<CategoryService>();
        services.AddScoped<ProductAttributeService>();
        services.AddScoped<ProductService>();

        return services;
    }
}
