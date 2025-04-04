using AdminDashboard.Client.Store.Breadcrumb;
using AdminDashboard.Client.Store.Categories;
using AdminDashboard.Client.Store.ProductAttributes;
using AdminDashboard.Client.Store.Products;
using Fluxor;
using Fluxor.Blazor.Web.ReduxDevTools;

namespace AdminDashboard.Client;

public static class CommonServices
{
    public static void AddCommonServices(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductAttributeService, ProductAttributeService>();
        services.AddScoped<IProductService, ProductService>();

        var currentAssembly = typeof(Program).Assembly;
        services.AddFluxor(options =>
        {
            options.ScanAssemblies(currentAssembly);
            options.UseReduxDevTools();
        });
    }
}