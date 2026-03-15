using MediatR;
using RestaurantOrderTracking.Domain.Common;

namespace RestaurantOrderTracking.Application.Feature.Payment.Commands.ConfirmWebhook
{
    /// <summary>
    /// Command yêu cầu đăng ký/xác nhận Webhook URL với PayOS.
    /// </summary>
    /// <param name="WebhookUrl">URL công khai (có https) mà PayOS sẽ POST tới khi có giao dịch.</param>
    public record ConfirmWebhookCommand(string WebhookUrl) : IRequest<Result<string>>;
}
