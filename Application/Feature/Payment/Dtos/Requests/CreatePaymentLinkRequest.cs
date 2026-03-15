namespace RestaurantOrderTracking.Application.Feature.Payment.Dtos
{
    /// <summary>
    /// DTO chứa dữ liệu đầu vào từ client để tạo link thanh toán PayOS.
    /// </summary>
    public class CreatePaymentLinkRequest
    {
        /// <summary>
        /// Mã hóa đơn (Bill) cần thanh toán.
        /// </summary>
        public Guid BillId { get; set; }

        /// <summary>
        /// URL PayOS sẽ redirect về khi người dùng bấm Hủy thanh toán.
        /// </summary>
        public string CancelUrl { get; set; } = string.Empty;

        /// <summary>
        /// URL PayOS sẽ redirect về sau khi thanh toán thành công.
        /// </summary>
        public string ReturnUrl { get; set; } = string.Empty;
    }
}
