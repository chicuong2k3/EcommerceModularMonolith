//using Billing.ApiContracts;
//using Billing.Domain.PaymentAggregate;
//using Common.Application;
//using Microsoft.AspNetCore.Mvc;

//namespace Ecommerce.Api.Controllers.Billing;

//[ApiController]
//[Route("api/[controller]")]
//public class PaymentsController : ControllerBase
//{
//    [HttpGet("{orderId}/payment-info")]
//    public async Task<IActionResult> GetPaymentInfo(Guid orderId)
//    {
//        //var payment = await paymentRepository.GetByOrderIdAsync(orderId);
//        //if (payment == null)
//        //    return NotFound();

//        //return Ok(new PaymentInfoResponse
//        //{
//        //    PaymentUrl = payment.PaymentUrl,
//        //    PaymentToken = payment.PaymentToken,
//        //    Status = payment.Status
//        //});

//        return Ok();
//    }

//    [HttpPost("callback")]
//    public async Task<IActionResult> HandleCallback([FromBody] PaymentCallbackRequest request)
//    {
//        //if (!VerifyCallback(request))
//        //    return BadRequest();

//        //var payment = await paymentRepository.GetByTokenAsync(request.PaymentToken);
//        //await payment.UpdateStatus(request.Status);

//        //await eventBus.PublishAsync(new PaymentStatusUpdatedEvent(
//        //    payment.OrderId,
//        //    payment.Status
//        //));

//        return Ok();
//    }
//}