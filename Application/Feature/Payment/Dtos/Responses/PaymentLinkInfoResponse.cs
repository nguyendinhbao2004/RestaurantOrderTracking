namespace RestaurantOrderTracking.Application.Feature.Payment.Dtos
{
    /// <summary>
    /// DTO chứa toàn bộ thông tin của một link thanh toán PayOS.
    /// </summary>
    public class PaymentLinkInfoResponse
    {
        /// <summary>PaymentLinkId do PayOS cấp.</summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>Mã đơn hàng nội bộ.</summary>
        public long OrderCode { get; set; }

        /// <summary>Tổng số tiền của link.</summary>
        public long Amount { get; set; }

        /// <summary>Số tiền đã được thanh toán.</summary>
        public long AmountPaid { get; set; }

        /// <summary>Số tiền còn lại chưa thanh toán.</summary>
        public long AmountRemaining { get; set; }

        /// <summary>Trạng thái link (PENDING, PAID, CANCELLED, EXPIRED).</summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>Thời điểm tạo link (ISO 8601).</summary>
        public string CreatedAt { get; set; } = string.Empty;

        /// <summary>Danh sách các giao dịch đã thực hiện với link này.</summary>
        public List<PaymentTransactionDetailDto> Transactions { get; set; } = new();
    }
}

