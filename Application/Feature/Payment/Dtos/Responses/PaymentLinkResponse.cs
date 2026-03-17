namespace RestaurantOrderTracking.Application.Feature.Payment.Dtos
{
    /// <summary>
    /// DTO chứa dữ liệu trả về sau khi tạo link thanh toán PayOS thành công.
    /// </summary>
    public class PaymentLinkResponse
    {
        /// <summary>Số tài khoản ngân hàng bin.</summary>
        public string Bin { get; set; } = string.Empty;

        /// <summary>Số tài khoản nhận tiền của kênh thanh toán.</summary>
        public string AccountNumber { get; set; } = string.Empty;

        /// <summary>Tên tài khoản nhận tiền.</summary>
        public string AccountName { get; set; } = string.Empty;

        /// <summary>Số tiền thanh toán.</summary>
        public long Amount { get; set; }

        /// <summary>Mô tả thanh toán (nội dung chuyển khoản).</summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>Mã đơn hàng nội bộ (orderCode).</summary>
        public long OrderCode { get; set; }

        /// <summary>Đơn vị tiền tệ, thường là VND.</summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>ID link thanh toán do PayOS cấp.</summary>
        public string PaymentLinkId { get; set; } = string.Empty;

        /// <summary>Trạng thái link thanh toán (PENDING, PAID, CANCELLED...).</summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>URL trang thanh toán để redirect người dùng tới.</summary>
        public string CheckoutUrl { get; set; } = string.Empty;

        /// <summary>Chuỗi mã QR cho VietQR.</summary>
        public string QrCode { get; set; } = string.Empty;
    }
}
