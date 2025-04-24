using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence.Repositories;

internal sealed class WriteProductRepository : IWriteProductRepository
{
    private readonly CatalogDbContext dbContext;

    public WriteProductRepository(CatalogDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Products.Where(p => p.Id == id)
                    .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Product?> GetByIdWithVariantsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Products.Where(p => p.Id == id)
                    .Include(p => p.Variants)
                    .ThenInclude(v => v.Attributes)
                    .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task RemoveAsync(Product product, CancellationToken cancellationToken = default)
    {
        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
