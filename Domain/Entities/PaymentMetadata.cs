namespace RestaurantOrderTracking.Domain.Entities
{
    public class PaymentMetadata
    {
        public string? Bin { get; set; }

        public string? AccountNumber { get; set; }

        public string? AccountName { get; set; }

        public string? Description { get; set; }

        public string? QrCode { get; set; }
    }
}
