namespace RestaurantOrderTracking.Application.Feature.Payment.Dtos
{
    /// <summary>
    /// DTO đại diện cho toàn bộ payload mà PayOS POST tới Webhook URL của hệ thống.
    /// </summary>
    public class PayOSWebhookPayload
    {
        public string Code { get; set; } = string.Empty;

        public string Desc { get; set; } = string.Empty;

        public bool Success { get; set; }

        /// <summary>Dữ liệu giao dịch chi tiết.</summary>
        public PayOSWebhookData Data { get; set; } = new();

        /// <summary>
        /// Chữ ký HMAC_SHA256 để kiểm tra tính toàn vẹn.
        /// </summary>
        public string Signature { get; set; } = string.Empty;
    }
}

