namespace RestaurantOrderTracking.Application.Feature.Payment.Dtos
{
    /// <summary>
    /// DTO đầu vào để hủy một link thanh toán PayOS.
    /// </summary>
    public class CancelPaymentLinkRequest
    {
        /// <summary>Mã OrderCode của link cần hủy (do hệ thống tạo và lưu trong PaymentTransaction).</summary>
        public long OrderCode { get; set; }

        /// <summary>Lý do hủy.</summary>
        public string? CancellationReason { get; set; }
    }
}
