using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Enums;
using System;

namespace RestaurantOrderTracking.Domain.Entities
{
    public class OrderItemLog : BaseEntities
    {
        public Guid OrderItemId { get; private set; }
        public virtual OrderItem OrderItem { get; private set; } = null!;

        public Guid? AccountId { get; private set; }
        public virtual Account? Account { get; private set; }

        public OrderItemStatus PreviousStatus { get; private set; }
        public OrderItemStatus NewStatus { get; private set; }
        public string? Notes { get; private set; }

        protected OrderItemLog() { }

        public OrderItemLog(Guid orderItemId, OrderItemStatus previousStatus, OrderItemStatus newStatus, Guid? accountId = null, string? notes = null)
        {
            OrderItemId = orderItemId;
            PreviousStatus = previousStatus;
            NewStatus = newStatus;
            AccountId = accountId;
            Notes = notes;
        }
    }
}
