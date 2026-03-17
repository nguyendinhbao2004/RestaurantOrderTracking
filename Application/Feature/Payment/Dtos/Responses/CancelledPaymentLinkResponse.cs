namespace RestaurantOrderTracking.Application.Feature.Payment.Dtos
{
    /// <summary>
    /// DTO chứa dữ liệu PayOS trả về sau khi hủy link thành công.
    /// </summary>
    public class CancelledPaymentLinkResponse
    {
        public string Id { get; set; } = string.Empty;
        public long OrderCode { get; set; }
        public long Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? CancellationReason { get; set; }
        public string? CanceledAt { get; set; }
    }
}
