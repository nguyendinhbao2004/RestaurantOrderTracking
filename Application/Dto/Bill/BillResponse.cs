using RestaurantOrderTracking.Domain.Enums;
using System;

namespace RestaurantOrderTracking.Application.Dto.Bill
{
    public class BillResponse
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string TableNumber { get; set; } = null!;
        public decimal Amount { get; set; }
        public decimal? Discount { get; set; }
        public decimal FinalAmount { get; set; }
        public string Status { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
        public DateTime? PaidAt { get; set; }
        public string CashierName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
