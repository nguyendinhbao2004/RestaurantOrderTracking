using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Enums;
using RestaurantOrderTracking.Domain.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Domain.Entities
{
    public class Order : BaseEntities
    {
        public Guid TableId { get; private set; }

        public Guid AccountId { get; private set; }
        public virtual Account Accounts { get; private set; }

        public virtual Table Table { get; private set; }

        public OrderStatus Status { get; private set; }

        private readonly List<OrderItems> _orderItems = new();
        public IReadOnlyCollection<OrderItems> OrderItems => _orderItems.AsReadOnly();

        public void AddItem(Guid ProductId, Guid AccountId, string note, string orderChanel)
        {
            if (Status == OrderStatus.Paid || Status == OrderStatus.Cancelled)
            {
                throw new InvalidOperationException("Cannot add items to a closed order.");
            }
            var orderItem = new OrderItems(this.Id, ProductId,orderChanel, note);
        }

        public Order(Guid tableId, OrderStatus status)
        {
            TableId = tableId;
            Status = status;
        }

        public void UpdateStatus(OrderStatus status)
        {
            Status = status;
        }
        public void DeleteStatus()
        {
            Status = OrderStatus.Deleted;
        }

        public void CheckOut()
        {
            if(_orderItems.Any(i => i.Status != OrderItemStatus.Served))
            {
                throw new InvalidOperationException("Cannot checkout. Some items are not served yet.");
            }
            Status = OrderStatus.Paid;
            // Raise Event để tạo Bill hoặc gửi Noti
            AddDomainEvent(new OrderCheckedOutEvent(this.Id));
        }
    }
}
