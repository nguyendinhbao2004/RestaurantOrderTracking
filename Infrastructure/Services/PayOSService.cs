using Microsoft.Extensions.Configuration;
using PayOS;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.Webhooks;
using RestaurantOrderTracking.Application.Common.Interface;
using System;
using System.Threading.Tasks;
using System.Text.Json;

namespace RestaurantOrderTracking.Infrastructure.Services
{
    public class PayOSService : IPayOSService
    {
        private readonly PayOSClient _payOSClient;

        public PayOSService(IConfiguration configuration)
        {
            var clientId = configuration["PAYOS_CLIENT_ID"] ?? throw new ArgumentNullException("PAYOS_CLIENT_ID is not configured.");
            var apiKey = configuration["PAYOS_API_KEY"] ?? throw new ArgumentNullException("PAYOS_API_KEY is not configured.");
            var checksumKey = configuration["PAYOS_CHECKSUM_KEY"] ?? throw new ArgumentNullException("PAYOS_CHECKSUM_KEY is not configured.");

            var options = new PayOSOptions
            {
                ClientId = clientId,
                ApiKey = apiKey,
                ChecksumKey = checksumKey
            };

            _payOSClient = new PayOSClient(options);
        }

        public async Task<string> CreatePaymentLink(long orderCode, int amount, string description, string cancelUrl, string returnUrl)
        {
            var request = new CreatePaymentLinkRequest
            {
                OrderCode = orderCode,
                Amount = amount,
                Description = description,
                CancelUrl = cancelUrl,
                ReturnUrl = returnUrl
            };

            var response = await _payOSClient.PaymentRequests.CreateAsync(request);
            // Assuming response has CheckoutUrl
            return response.CheckoutUrl;
        }

        public async Task<long?> VerifyPaymentWebhook(string webhookBody)
        {
            try
            {
                var webhook = JsonSerializer.Deserialize<Webhook>(webhookBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (webhook == null) return null;

                var verifiedData = await _payOSClient.Webhooks.VerifyAsync(webhook);
                
                if (verifiedData.Code == "00")
                {
                    return verifiedData.OrderCode;
                }
                
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Webhook verification failed: {ex.Message}");
                return null;
            }
        }
    }
}
