

namespace Billing.Application.Payments.Commands;

public record CreatePayment(Guid OrderId, Guid CustomerId, decimal TotalAmount) : ICommand;

internal class CreatePaymentHandler(
    IPaymentRepository paymentRepository)
    : ICommandHandler<CreatePayment>
{
    public async Task<Result> Handle(CreatePayment command, CancellationToken cancellationToken)
    {
        var totalAmountCreationResult = Money.FromDecimal(command.TotalAmount);
        if (totalAmountCreationResult.IsFailed)
            return totalAmountCreationResult.ToResult();

        var payment = new Payment(command.OrderId, command.CustomerId, totalAmountCreationResult.Value);
        await paymentRepository.AddAsync(payment, cancellationToken);

        return Result.Ok();
    }
}