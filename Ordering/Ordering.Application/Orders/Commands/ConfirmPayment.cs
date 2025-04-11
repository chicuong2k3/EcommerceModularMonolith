namespace Ordering.Application.Orders.Commands;

public record ConfirmPayment(
    Guid OrderId,
    string PaymentTransactionId) : ICommand;


internal class ConfirmPaymentHandler(
    IOrderRepository orderRepository)
    : ICommandHandler<ConfirmPayment>
{
    public async Task<Result> Handle(ConfirmPayment command, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(command.OrderId, cancellationToken);
        if (order == null)
            return Result.Fail(new NotFoundError($"Order with id '{command.OrderId}' not found"));

        if (order.Status != OrderStatus.PendingPayment)
        {
            return Result.Fail(new ValidationError("Order is not pending payment"));
        }

        //var paymentResult = await paymentGateway.Confirm(command.PaymentTransactionId);
        var paymentResult = Result.Ok(); // Simulate payment confirmation
        if (!paymentResult.IsSuccess)
            return Result.Fail("Payment confirmation failed");

        var confirmResult = order.MarkAsPaid();
        if (confirmResult.IsFailed)
            return confirmResult;

        var result = order.();
        if (result.IsFailed)
            return result;

        await orderRepository.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}