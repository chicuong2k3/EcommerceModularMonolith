using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Core.Persistence;

public static class DatabaseExtensions
{
    public static async Task MigrateCatalogDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<CatalogDbContext>();

        if (context.Database.GetPendingMigrations().Any())
        {
            await context.Database.MigrateAsync();
        }

    }
}
