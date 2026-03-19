using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantOrderTracking.Application.Feature.Payment.Commands.CancelPaymentLink;
using RestaurantOrderTracking.Application.Feature.Payment.Commands.ConfirmWebhook;
using RestaurantOrderTracking.Application.Feature.Payment.Commands.CreatePaymentLink;
using RestaurantOrderTracking.Application.Feature.Payment.Commands.ProcessWebhook;
using RestaurantOrderTracking.Application.Feature.Payment.Dtos;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using RestaurantOrderTracking.Application.Feature.Payment.Queries.GetPaymentInfo;
using System.Threading.Tasks;

namespace RestaurantOrderTracking.WebApi.Controllers
{
    /// <summary>
    /// Controller quản lý toàn bộ luồng Nhận tiền qua PayOS.
    /// Bao gồm: Tạo link, Lấy thông tin, Hủy link, Xử lý Webhook, Đăng ký Webhook URL.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ══════════════════════════════════════════════════════════════════════
        // 1. TẠO LINK THANH TOÁN
        // POST api/payment/create-link
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Tạo link thanh toán PayOS cho một hóa đơn chưa thanh toán.
        /// </summary>
        /// <remarks>
        /// Yêu cầu: Bill phải tồn tại và có trạng thái "unpaid".
        /// Service sẽ tự động tính chữ ký HMAC_SHA256 trước khi gửi lên PayOS.
        /// </remarks>
        [HttpPost("create-link")]
        public async Task<IActionResult> CreatePaymentLink([FromBody] CreatePaymentLinkRequest dto)
        {
            var command = new CreatePaymentLinkCommand(
                dto.BillId,
                dto.CancelUrl,
                dto.ReturnUrl,
                ResolveCurrentAccountId());
            var result = await _mediator.Send(command);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        // ══════════════════════════════════════════════════════════════════════
        // 2. LẤY THÔNG TIN LINK THANH TOÁN
        // GET api/payment/info/{orderCode}
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Lấy thông tin trực tiếp từ PayOS về trạng thái link thanh toán theo orderCode nội bộ.
        /// </summary>
        [HttpGet("info/{orderCode:long}")]
        public async Task<IActionResult> GetPaymentInfo([FromRoute] long orderCode)
        {
            var query = new GetPaymentInfoQuery(orderCode);
            var result = await _mediator.Send(query);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        // ══════════════════════════════════════════════════════════════════════
        // 3. HỦY LINK THANH TOÁN
        // POST api/payment/cancel
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Hủy một link thanh toán PayOS đang ở trạng thái PENDING.
        /// Cập nhật trạng thái cả PaymentTransaction lẫn Bill về CANCELLED.
        /// </summary>
        [HttpPost("cancel")]
        public async Task<IActionResult> CancelPaymentLink([FromBody] CancelPaymentLinkRequest dto)
        {
            var command = new CancelPaymentLinkCommand(dto.OrderCode, dto.CancellationReason);
            var result = await _mediator.Send(command);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        // ══════════════════════════════════════════════════════════════════════
        // 4. XỬ LÝ WEBHOOK TỪ PAYOS
        // POST api/payment/webhook
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Endpoint nhận thông báo thanh toán từ PayOS (Webhook).
        /// </summary>
        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook([FromBody] PayOSWebhookPayload payload)
        {
            var command = new ProcessWebhookCommand(payload);
            var result = await _mediator.Send(command);

            return Ok(new { success = result.Succeeded, message = result.Message ?? result.Errors?.FirstOrDefault() });
        }

        // ══════════════════════════════════════════════════════════════════════
        // 5. ĐĂNG KÝ WEBHOOK URL VỚI PAYOS
        // POST api/payment/confirm-webhook
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Đăng ký hoặc cập nhật Webhook URL với PayOS.
        /// </summary>
        [HttpPost("confirm-webhook")]
        public async Task<IActionResult> ConfirmWebhook([FromBody] ConfirmWebhookRequest dto)
        {
            var command = new ConfirmWebhookCommand(dto.WebhookUrl);
            var result = await _mediator.Send(command);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        private Guid? ResolveCurrentAccountId()
        {
            if (User?.Identity?.IsAuthenticated != true)
                return null;

            var accountIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                                 ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                                 ?? User.FindFirstValue("sub");

            return Guid.TryParse(accountIdClaim, out var accountId) ? accountId : null;
        }
    }
}
