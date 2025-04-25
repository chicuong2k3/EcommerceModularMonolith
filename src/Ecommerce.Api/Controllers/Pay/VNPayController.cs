using Pay.Core.Repositories;
using Pay.Core.Services;
using Pay.Core.ValueObjects;
using System.Security.Cryptography;
using System.Text;

namespace Ecommerce.Api.Controllers.Pay;

[ApiController]
[Route("api/payments/vnpay")]
public class VNPayController : ControllerBase
{
    private readonly IPaymentRepository paymentRepository;
    private readonly VNPayConfig config;
    private readonly ILogger<VNPayController> logger;

    public VNPayController(IPaymentRepository paymentRepository,
                           VNPayConfig config,
                           ILogger<VNPayController> logger)
    {
        this.paymentRepository = paymentRepository;
        this.config = config;
        this.logger = logger;
    }

    [HttpPost("ipn")]
    public async Task<IActionResult> IPN([FromForm] Dictionary<string, string> vnpParams)
    {
        // Verify checksum
        var secureHash = vnpParams["vnp_SecureHash"];
        vnpParams.Remove("vnp_SecureHash");
        vnpParams.Remove("vnp_SecureHashType");

        var signData = string.Join("&", vnpParams
            .OrderBy(x => x.Key)
            .Select(x => $"{x.Key}={x.Value}"));

        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(config.HashSecret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(signData));
        var mySecureHash = BitConverter.ToString(hash).Replace("-", "").ToLower();

        if (secureHash != mySecureHash)
            return BadRequest("Invalid checksum");

        // Process payment result
        var orderId = Guid.Parse(vnpParams["vnp_TxnRef"]);
        var responseCode = vnpParams["vnp_ResponseCode"];
        var transactionNo = vnpParams["vnp_TransactionNo"];

        var payment = await paymentRepository.GetPaymentByOrderIdAsync(orderId);
        if (payment == null)
            return NotFound("The payment not found");

        Result processCallbackResult = Result.Ok();
        if (responseCode == "00")
        {
            processCallbackResult = payment.ProcessCallback(
                transactionNo,
                PaymentResponseCode.Success);
        }
        else
        {
            processCallbackResult = payment.ProcessCallback(
                transactionNo,
                PaymentResponseCode.Failed);
        }

        if (processCallbackResult.IsFailed)
            return processCallbackResult.ToActionResult();

        await paymentRepository.SaveChangesAsync();

        return Ok("{\"RspCode\":\"00\",\"Message\":\"Confirm Success\"}");
    }
}
