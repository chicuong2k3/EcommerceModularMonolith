using Microsoft.Extensions.Logging;
using MediatR;
using Pay.Core.Repositories;
using Pay.Core.ValueObjects;
using Shared.Abstractions.Application;
using Ordering.Contracts;
using Pay.Core.Services;
using Pay.Core.Commands;

namespace Pay.Core.EventHandlers.IntegrationEvents;

internal class ProcessPaymentOnOrderPlacedForOnlinePayment
    : IntegrationEventHandler<OrderPlacedForOnlinePaymentIntegrationEvent>
{
    private readonly ILogger<ProcessPaymentOnOrderPlacedForOnlinePayment> logger;
    private readonly IEventBus eventBus;
    private readonly IPaymentGatewayFactory paymentGatewayFactory;
    private readonly IMediator mediator;
    private readonly IPaymentRepository paymentRepository;
    private readonly string returnUrl;

    public ProcessPaymentOnOrderPlacedForOnlinePayment(
        ILogger<ProcessPaymentOnOrderPlacedForOnlinePayment> logger,
        IEventBus eventBus,
        IPaymentGatewayFactory paymentGatewayFactory,
        IMediator mediator,
        IPaymentRepository paymentRepository,
        PaymentSettings paymentSettings)
    {
        this.logger = logger;
        this.eventBus = eventBus;
        this.paymentGatewayFactory = paymentGatewayFactory;
        this.mediator = mediator;
        this.paymentRepository = paymentRepository;
        returnUrl = paymentSettings.ReturnUrl;
    }

    public override async Task Handle(OrderPlacedForOnlinePaymentIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        try
        {
            var createPaymentResult = await mediator.Send(
                new CreatePayment(integrationEvent.OrderId, integrationEvent.CustomerId, integrationEvent.TotalAmount),
                cancellationToken);

            if (createPaymentResult.IsFailed)
            {
                logger.LogError("Failed to create payment for order {OrderId}: {Errors}",
                    integrationEvent.OrderId, string.Join(", ", createPaymentResult.Errors));
                throw new Exception("Payment creation failed");
            }

            var paymentMethod = Enum.Parse<PaymentMethod>(integrationEvent.PaymentMethod);

            var gateway = paymentGatewayFactory.CreateGateway(paymentMethod);
            var payment = await paymentRepository.GetPaymentByOrderIdAsync(integrationEvent.OrderId, cancellationToken);

            if (payment == null)
            {
                logger.LogError("Payment not found for order {OrderId}", integrationEvent.OrderId);
                throw new Exception("Payment not found");
            }

            var paymentUrlResult = await gateway.CreatePaymentUrlAsync(
                payment,
                returnUrl,
                cancellationToken);

            if (paymentUrlResult.IsFailed)
            {
                logger.LogError("Failed to create payment URL for order {OrderId}: {Errors}",
                    integrationEvent.OrderId, string.Join(", ", paymentUrlResult.Errors));
                throw new Exception("Payment URL creation failed");
            }

            payment.SetPaymentUrlAndToken(
                paymentUrlResult.Value.PaymentUrl,
                paymentUrlResult.Value.PaymentToken);

            await paymentRepository.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                            "Payment URL generated for order {OrderId}: {PaymentUrl}",
                            integrationEvent.OrderId,
                            paymentUrlResult.Value.PaymentUrl);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing payment for order {OrderId}", integrationEvent.OrderId);
            throw;
        }
    }
}
