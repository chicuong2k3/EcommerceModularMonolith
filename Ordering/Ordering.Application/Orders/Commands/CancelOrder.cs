namespace Ordering.Application.Orders.Commands;

public record CancelOrder(Guid OrderId) : ICommand;

public class CancelOrderHandler(IOrderRepository orderRepository)
    : ICommandHandler<CancelOrder>
{
    public async Task<Result> Handle(CancelOrder command, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(command.OrderId);
        if (order == null)
            return Result.Fail(new NotFoundError($"Order with id '{command.OrderId}' not found"));

        var result = order.Cancel();

        if (result.IsFailed)
            return result;

        await orderRepository.SaveChangesAsync(cancellationToken);
        return result;
    }
}