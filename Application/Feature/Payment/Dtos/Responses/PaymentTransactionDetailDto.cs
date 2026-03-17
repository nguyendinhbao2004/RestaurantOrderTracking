namespace RestaurantOrderTracking.Application.Feature.Payment.Dtos
{
    /// <summary>
    /// DTO chứa thông tin chi tiết của một giao dịch trong link thanh toán.
    /// </summary>
    public class PaymentTransactionDetailDto
    {
        public string Reference { get; set; } = string.Empty;
        public long Amount { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime TransactionDateTime { get; set; }
        public string? VirtualAccountName { get; set; }
        public string? VirtualAccountNumber { get; set; }
        public string? CounterAccountBankId { get; set; }
        public string? CounterAccountBankName { get; set; }
        public string? CounterAccountName { get; set; }
        public string? CounterAccountNumber { get; set; }
    }
}
