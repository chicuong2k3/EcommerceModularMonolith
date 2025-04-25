using Pay.Core.ValueObjects;

namespace Pay.Core.Services;

public interface IPaymentGatewayFactory
{
    IPaymentGateway CreateGateway(PaymentProvider paymentProvider);
}