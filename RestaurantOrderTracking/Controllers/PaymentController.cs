using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantOrderTracking.Application.Feature.Payment.Commands.CreatePaymentLink;
using RestaurantOrderTracking.Application.Feature.Payment.Commands.ProcessWebhook;
using System.Text.Json;
using System.Threading.Tasks;

namespace RestaurantOrderTracking.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a payment link using PayOS for an unpaid bill.
        /// </summary>
        /// <param name="command">Command containing BillId, CancelUrl, and ReturnUrl</param>
        /// <returns>A checkout URL to redirect the user to.</returns>
        [HttpPost("create-payment-link")]
        public async Task<IActionResult> CreatePaymentLink([FromBody] CreatePaymentLinkCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Webhook endpoint for PayOS to send payment result notifications.
        /// </summary>
        /// <param name="webhookData">The raw JSON data sent by PayOS</param>
        /// <returns>Result of processing.</returns>
        [HttpPost("payos-webhook")]
        public async Task<IActionResult> PayOSWebhook([FromBody] object webhookData)
        {
            string webhookBody = JsonSerializer.Serialize(webhookData);
            var command = new ProcessWebhookCommand(webhookBody);
            var result = await _mediator.Send(command);

            if (result.Succeeded)
                return Ok(new { success = true, message = result.Message });

            return BadRequest(new { success = false, message = result.Message ?? result.Errors?.FirstOrDefault() });
        }
    }
}
