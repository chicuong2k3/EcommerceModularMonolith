using FluentResults;
using Pay.Core.Entities;
using Pay.Core.Repositories;
using Pay.Core.ValueObjects;
using Shared.Abstractions.Application;
using Shared.Abstractions.Core;

namespace Pay.Core.Commands;

public record CreatePayment(
    Guid Id,
    Guid OrderId,
    Guid CustomerId,
    decimal TotalAmount,
    string PaymentProvider) : ICommand;

internal class CreatePaymentHandler(
    IPaymentRepository paymentRepository)
    : ICommandHandler<CreatePayment>
{
    public async Task<Result> Handle(CreatePayment command, CancellationToken cancellationToken)
    {
        var totalAmountCreationResult = Money.FromDecimal(command.TotalAmount);
        if (totalAmountCreationResult.IsFailed)
            return totalAmountCreationResult.ToResult();

        if (!Enum.TryParse<PaymentProvider>(command.PaymentProvider, out var paymentProvider))
            return Result.Fail(new ValidationError($"Invalid payment provider: {command.PaymentProvider}"));

        var payment = new Payment(
                command.Id,
                command.OrderId,
                command.CustomerId,
                totalAmountCreationResult.Value,
                paymentProvider);
        await paymentRepository.AddAsync(payment, cancellationToken);

        return Result.Ok();
    }
}