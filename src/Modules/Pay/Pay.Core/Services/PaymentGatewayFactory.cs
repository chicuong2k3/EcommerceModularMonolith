using Microsoft.Extensions.DependencyInjection;
using Pay.Core.ValueObjects;

namespace Pay.Core.Services;

internal class PaymentGatewayFactory : IPaymentGatewayFactory
{
    private readonly IServiceProvider serviceProvider;

    public PaymentGatewayFactory(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public IPaymentGateway CreateGateway(PaymentProvider paymentProvider)
    {
        return paymentProvider switch
        {
            PaymentProvider.Momo => serviceProvider.GetRequiredService<MomoGateway>(),
            PaymentProvider.VNPay => serviceProvider.GetRequiredService<VNPayGateway>(),
            _ => throw new NotSupportedException($"Payment provider {paymentProvider} is not supported.")
        };
    }
}
