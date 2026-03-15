namespace RestaurantOrderTracking.Application.Feature.Payment.Helpers
{
    /// <summary>
    /// DTO chứa danh sách các field dùng để tính chữ ký HMAC_SHA256 khi tạo link thanh toán.
    /// </summary>
    public class PaymentSignatureData
    {
        /// <summary>Số tiền thanh toán.</summary>
        public long Amount { get; set; }

        public string CancelUrl { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public long OrderCode { get; set; }

        public string ReturnUrl { get; set; } = string.Empty;

        public string ToSignatureString()
        {
            return $"amount={Amount}" +
                   $"&cancelUrl={CancelUrl}" +
                   $"&description={Description}" +
                   $"&orderCode={OrderCode}" +
                   $"&returnUrl={ReturnUrl}";
        }
    }
}
