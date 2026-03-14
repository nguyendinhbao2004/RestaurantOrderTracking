using System.Threading.Tasks;

namespace RestaurantOrderTracking.Application.Common.Interface
{
    public interface IPayOSService
    {
        Task<string> CreatePaymentLink(long orderCode, int amount, string description, string cancelUrl, string returnUrl);
        // Receives webhook payload as JSON string. Returns the OrderCode if successful and parsed.
        Task<long?> VerifyPaymentWebhook(string webhookBody);
    }
}
