using MediatR;
using RestaurantOrderTracking.Domain.Common;

namespace RestaurantOrderTracking.Application.Feature.Payment.Commands.ProcessWebhook
{
    public record ProcessWebhookCommand(
        string WebhookBody
    ) : IRequest<Result<string>>;
}
