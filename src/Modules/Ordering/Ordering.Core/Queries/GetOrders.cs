using FluentResults;
using Ordering.Core.ReadModels;
using Ordering.Core.Repositories;
using Ordering.Core.ValueObjects;
using Shared.Abstractions.Application;
using Shared.Abstractions.Core;

namespace Ordering.Core.Queries;

public record GetOrders(
    int PageNumber,
    int PageSize,
    Guid? CustomerId,
    string? OrderStatus,
    DateTime? StartOrderDate,
    DateTime? EndOrderDate,
    decimal? MinTotal,
    decimal? MaxTotal,
    string? PaymentMethod) : IQuery<PaginationResult<OrderReadModel>>;

internal class GetOrdersHandler(IReadOrderRepository orderRepository)
    : IQueryHandler<GetOrders, PaginationResult<OrderReadModel>>
{
    public async Task<Result<PaginationResult<OrderReadModel>>> Handle(GetOrders query, CancellationToken cancellationToken)
    {
        var specification = new GetOrdersSpecification()
        {
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            CustomerId = query.CustomerId,
            OrderStatus = Enum.TryParse(query.OrderStatus, out OrderStatus orderStatus) ? orderStatus : null,
            StartOrderDate = query.StartOrderDate,
            EndOrderDate = query.EndOrderDate,
            MinTotal = query.MinTotal,
            MaxTotal = query.MaxTotal,
            PaymentMethod = Enum.TryParse(query.PaymentMethod, out PaymentMethod paymentMethod) ? paymentMethod : null
        };

        var result = await orderRepository.GetAllAsync(specification, cancellationToken);
        return result;
    }
}
