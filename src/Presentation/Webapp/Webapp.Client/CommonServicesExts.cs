using Sysinfocus.AspNetCore.Components;
using TimeWarp.State;
using Webapp.Client.Features.Category;

namespace Webapp.Client;

public static class CommonServicesExts
{
    public static void RegisterCommonServices(this IServiceCollection services)
    {
        services.AddSysinfocus(false);

        services.AddTimeWarpState();
        services.AddScoped<CategoryService>();


    }
}
