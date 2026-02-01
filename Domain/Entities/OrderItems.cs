using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Domain.Entities
{
    public class OrderItems : BaseEntities
    {
        public Guid OrderId { get; private set; }
        public virtual Order Order { get; private set; }

        public Guid ProductId { get; private set; }
        public virtual Product Products { get; private set; }

        public Guid ChefAccountId { get; private set; }
        public virtual Account ChefAccounts { get; private set; }

        public Guid WaiterAccountId { get; private set; }
        public virtual Account WaiterAccounts { get; private set; }

        public string OrderChanel { get; private set; }

        public string Note { get; private set; }

        public OrderItemStatus Status { get; private set; }

        public DateTime Confirmed_At { get; private set; }

        public DateTime Kitchen_Confirmed_At { get; private set; }

        public DateTime Kitchen_Finish_At { get; private set; }

        public DateTime Waiter_Arrive_At { get; private set; }

        public DateTime Waiter_Served_At { get; set; }

        public OrderItems(Guid orderId, Guid productId, string orderChanel, string note)
        {
            OrderId = orderId;
            ProductId = productId;
            OrderChanel = orderChanel;
            Note = note;
            Status = OrderItemStatus.Pending;
        }

        public void ConfirmByKitchen(Guid chefId)
        {
            ChefAccountId = chefId;
            Status = OrderItemStatus.Cooking;
            Kitchen_Confirmed_At = DateTime.UtcNow;
        }

        public void ConfirmByWaiter(Guid waiterId)
        {
            WaiterAccountId = waiterId;
            Status = OrderItemStatus.Served;
            Waiter_Arrive_At = DateTime.UtcNow;
        }

    }
}
