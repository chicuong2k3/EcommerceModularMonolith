using Pay.Core.Services;
using Pay.Core.ValueObjects;

namespace Pay.Infrastructure.Gateways;

internal class PaymentGatewayFactory : IPaymentGatewayFactory
{
    public IPaymentGateway CreateGateway(PaymentMethod paymentMethod)
    {
        return paymentMethod switch
        {
            PaymentMethod.Momo => new MomoGateway(),
            _ => throw new NotSupportedException($"Payment method {paymentMethod} is not supported.")
        };
    }
}
