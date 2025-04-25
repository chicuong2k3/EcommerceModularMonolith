using Microsoft.EntityFrameworkCore;
using Ordering.Core.Entities;
using Ordering.Core.Persistence;
using Ordering.Core.Repositories;

namespace Ordering.Core.Persistence.Repositories;

internal class OrderRepository : IOrderRepository
{
    private readonly OrderingDbContext dbContext;

    public OrderRepository(OrderingDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
    }

    public async Task RemoveAsync(Order order, CancellationToken cancellationToken = default)
    {
        dbContext.Orders.Remove(order);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
