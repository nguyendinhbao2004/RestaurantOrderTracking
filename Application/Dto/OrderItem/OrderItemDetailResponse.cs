using System;

namespace RestaurantOrderTracking.Application.Dto.OrderItem
{
    public class OrderItemDetailResponse
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid? TableId { get; set; }
        public string? TableNumber { get; set; }

        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal ProductPrice { get; set; }

        public Guid? ChefAccountId { get; set; }
        public string? ChefName { get; set; }

        public Guid? WaiterAccountId { get; set; }
        public string? WaiterName { get; set; }

        public string OrderChannel { get; set; } = null!;
        public string? Note { get; set; }
        public string Status { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
