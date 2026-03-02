using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Enums;
using System;

namespace RestaurantOrderTracking.Domain.Entities
{
    public class OrderItem : BaseEntities
    {
        public Guid OrderId { get; private set; }
        public virtual Order Order { get; private set; } = null!;

        public Guid ProductId { get; private set; }
        public virtual Product Product { get; private set; } = null!;

        public Guid? ChefAccountId { get; private set; }
        public virtual Account? ChefAccount { get; private set; }

        public Guid? WaiterAccountId { get; private set; }
        public virtual Account? WaiterAccount { get; private set; }

        public string OrderChannel { get; private set; } = null!;
        public string? Note { get; private set; }
        
        public OrderItemStatus Status { get; private set; }

       

        protected OrderItem() { }

        public OrderItem(Guid orderId, Guid productId, string orderChannel, string? note = null, int quantity = 1)
        {
            OrderId = orderId;
            ProductId = productId;
            OrderChannel = orderChannel;
            Note = note;
            Status = OrderItemStatus.Pending;
        }

       
        public void Cancel()
        {
            Status = OrderItemStatus.Cancelled;
        }

        public void UpdateNote(string? note)
        {
            Note = note;
        }
    }
}
