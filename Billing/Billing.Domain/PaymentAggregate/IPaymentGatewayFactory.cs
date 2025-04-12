namespace Billing.Domain.PaymentAggregate;

public interface IPaymentGatewayFactory
{
    IPaymentGateway CreateGateway(PaymentMethod paymentMethod);
}