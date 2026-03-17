namespace RestaurantOrderTracking.Application.Feature.Payment.Dtos
{
    /// <summary>
    /// DTO nhận webhookUrl từ request body cho endpoint confirm-webhook.
    /// </summary>
    public record ConfirmWebhookRequest(string WebhookUrl);
}
