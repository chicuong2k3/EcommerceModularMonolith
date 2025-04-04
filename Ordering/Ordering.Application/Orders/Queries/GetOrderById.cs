namespace Ordering.Application.Orders.Queries;

public record GetOrderById(Guid OrderId) : IQuery<Order>;

internal sealed class GetOrderByIdHandler(IOrderRepository orderRepository)
    : IQueryHandler<GetOrderById, Order>
{
    public async Task<Result<Order>> Handle(GetOrderById query, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(query.OrderId, cancellationToken);
        if (order == null)
            return Result.Fail(new NotFoundError($"The order with id '{query.OrderId}' not found"));

        return Result.Ok(order);
    }
}