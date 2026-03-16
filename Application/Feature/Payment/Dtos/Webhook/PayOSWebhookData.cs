using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace RestaurantOrderTracking.Application.Feature.Payment.Dtos
{
    public class PayOSWebhookData
    {
        public long OrderCode { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
        public string TransactionDateTime { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public string PaymentLinkId { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;
        public string Desc { get; set; } = string.Empty;
        public string? CounterAccountBankId { get; set; }
        public string? CounterAccountBankName { get; set; }
        public string? CounterAccountName { get; set; }
        public string? CounterAccountNumber { get; set; }
        public string? VirtualAccountName { get; set; }
        public string? VirtualAccountNumber { get; set; }

        // get all other fields from PayOS Webhook payload that are not defined above
        [JsonExtensionData]
        public Dictionary<string, System.Text.Json.JsonElement>? AdditionalData { get; set; }
    }
}