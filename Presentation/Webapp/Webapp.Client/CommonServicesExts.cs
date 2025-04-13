using Sysinfocus.AspNetCore.Components;

namespace Webapp.Client;

public static class CommonServicesExts
{
    public static void RegisterCommonServices(this IServiceCollection services)
    {
        services.AddSysinfocus(false);
    }
}
