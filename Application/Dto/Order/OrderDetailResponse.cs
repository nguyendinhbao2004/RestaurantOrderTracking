using RestaurantOrderTracking.Application.Dto.OrderItem;
using System;
using System.Collections.Generic;

namespace RestaurantOrderTracking.Application.Dto.Order
{
    public class OrderDetailResponse
    {
        public Guid Id { get; set; }

        public Guid? TableId { get; set; }
        public string? TableNumber { get; set; }

        public string OrderType { get; set; } = null!;
        public string Status { get; set; } = null!;

        public Guid? WaiterId { get; set; }
        public string? WaiterName { get; set; }

        public Guid? CustomerId { get; set; }
        public string? CustomerName { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<OrderItemDetailResponse> OrderItems { get; set; } = new();
    }
}
