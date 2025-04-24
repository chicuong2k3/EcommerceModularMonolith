//using MediatR;
//using Ordering.Application.Products.Commands;
//using Ordering.Domain.OrderAggregate.Events;

//namespace Ordering.Application.EventHandlers.DomainEvents;

//internal class ReduceProductQuantityOnOrderPlaced(IMediator mediator)
//    : DomainEventHandler<OrderPlaced>
//{
//    public override async Task Handle(OrderPlaced domainEvent, CancellationToken cancellationToken = default)
//    {
//        foreach (var orderItem in domainEvent.OrderItems)
//        {
//            var result = await mediator.Send(
//                new ReduceProductQuantity(orderItem.ProductId,
//                                          orderItem.ProductVariantId,
//                                          orderItem.Quantity), cancellationToken);

//            if (result.IsFailed)
//                return;
//        }
//    }
//}
