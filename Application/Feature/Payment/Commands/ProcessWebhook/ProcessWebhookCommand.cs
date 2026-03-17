using MediatR;
using RestaurantOrderTracking.Application.Feature.Payment.Dtos;
using RestaurantOrderTracking.Domain.Common;

namespace RestaurantOrderTracking.Application.Feature.Payment.Commands.ProcessWebhook
{
    /// <summary>
    /// Command nhận payload webhook đã deserialize từ PayOS.
    /// </summary>
    /// <param name="Payload">Toàn bộ payload JSON từ PayOS (đã được bind bởi ASP.NET).</param>
    public record ProcessWebhookCommand(PayOSWebhookPayload Payload) : IRequest<Result<string>>;
}
