using FluentResults;
using Ordering.Core.ReadModels;
using Ordering.Core.Repositories;
using Shared.Abstractions.Application;
using Shared.Abstractions.Core;

namespace Ordering.Core.Queries;

public record GetOrderById(Guid OrderId) : IQuery<OrderReadModel>;

internal sealed class GetOrderByIdHandler(IReadOrderRepository orderRepository)
    : IQueryHandler<GetOrderById, OrderReadModel>
{
    public async Task<Result<OrderReadModel>> Handle(GetOrderById query, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(query.OrderId, cancellationToken);

        if (order == null)
            return Result.Fail(new NotFoundError($"The order with id '{query.OrderId}' not found"));

        return Result.Ok(order);
    }
}