using Catalog.Core.Entities;
using Catalog.Core.Persistence;
using Catalog.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Core.Persistence.Repositories;

internal class ProductAttributeRepository : IProductAttributeRepository
{
    private readonly CatalogDbContext dbContext;

    public ProductAttributeRepository(CatalogDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(ProductAttribute attribute, CancellationToken cancellationToken = default)
    {
        dbContext.ProductAttributes.Add(attribute);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<List<ProductAttribute>> GetAttributesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.ProductAttributes.ToListAsync(cancellationToken);
    }

    public async Task<ProductAttribute?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var attribute = await dbContext.ProductAttributes.FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());
        return attribute;
    }

    public async Task RemoveAsync(ProductAttribute attribute, CancellationToken cancellationToken = default)
    {
        dbContext.ProductAttributes.Remove(attribute);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<ProductAttribute>> GetByIdsAsync(List<Guid> ids, CancellationToken cancellationToken = default)
    {
        var attributes = await dbContext.ProductAttributes.Where(a => ids.Contains(a.Id)).ToListAsync();
        return attributes;
    }
}
