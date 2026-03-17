using RestaurantOrderTracking.Application.Feature.Payment.Dtos;
using System.Threading.Tasks;

namespace RestaurantOrderTracking.Application.Common.Interface
{
    public interface IPayOSService
    {
        Task<PaymentLinkResponse> CreatePaymentLinkAsync(long orderCode, int amount, string description, string cancelUrl, string returnUrl);

        Task<PaymentLinkInfoResponse> GetPaymentLinkInfoAsync(string paymentLinkId);

        Task<CancelledPaymentLinkResponse> CancelPaymentLinkAsync(string paymentLinkId, string? cancellationReason = null);

        PayOSWebhookData? VerifyAndExtractWebhookData(PayOSWebhookPayload payload);

        Task<bool> ConfirmWebhookUrlAsync(string webhookUrl);

        string ComputeHmacSha256(string data);
    }
}