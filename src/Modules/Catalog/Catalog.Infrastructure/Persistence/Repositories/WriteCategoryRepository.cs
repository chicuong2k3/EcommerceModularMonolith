using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence.Repositories;

internal sealed class WriteCategoryRepository : IWriteCategoryRepository
{
    private readonly CatalogDbContext dbContext;

    public WriteCategoryRepository(CatalogDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Categories.ToListAsync(cancellationToken);
    }

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Categories.FindAsync(id, cancellationToken);
    }

    public Task<Category?> GetByIdWithSubCategoriesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Categories
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task RemoveAsync(Category category, CancellationToken cancellationToken = default)
    {
        dbContext.Categories.Remove(category);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
