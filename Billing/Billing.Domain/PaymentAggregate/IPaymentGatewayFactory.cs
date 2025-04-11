namespace Billing.Domain.PaymentAggregate;

public interface IPaymentGatewayFactory
{
    IPaymentGateway CreateGateway(string paymentMethod);
}