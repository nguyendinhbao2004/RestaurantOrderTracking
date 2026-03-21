using System;

namespace RestaurantOrderTracking.Application.Feature.Payment.Dtos
{
    public class PaymentMetadataResponse
    {
        public string? Bin { get; set; }

        public string? AccountNumber { get; set; }

        public string? AccountName { get; set; }

        public string? Description { get; set; }

        public string? QrCode { get; set; }
    }

    public class PaymentInfoByOrderIdResponse
    {
        public Guid BillId { get; set; }

        public long OrderCode { get; set; }

        public decimal Amount { get; set; }

        public string Status { get; set; } = string.Empty;

        public PaymentMetadataResponse? PaymentMetadata { get; set; }
    }
}
