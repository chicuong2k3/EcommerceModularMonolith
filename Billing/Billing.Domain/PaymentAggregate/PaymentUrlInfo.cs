namespace Billing.Domain.PaymentAggregate;

public record PaymentUrlInfo(
    string PaymentUrl,    
    string PaymentToken,  
    string? QrCode       
);
