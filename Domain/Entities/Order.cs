using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Enums;
using RestaurantOrderTracking.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantOrderTracking.Domain.Entities
{
    public class Order : BaseEntities
    {
        public Guid TableId { get; private set; }
        public virtual Table Table { get; private set; } = null!;

        public Guid AccountId { get; private set; }
        public virtual Account Account { get; private set; } = null!;

        public OrderStatus Status { get; private set; }

        private readonly List<OrderItem> _orderItems = new();
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

        protected Order() { }

        public Order(Guid tableId, Guid accountId, OrderStatus status = OrderStatus.Pending)
        {
            TableId = tableId;
            AccountId = accountId;
            Status = status;
        }

        public void AddItem(Guid productId, Guid accountId, string note, string orderChannel)
        {
            if (Status == OrderStatus.Paid || Status == OrderStatus.Cancelled)
            {
                throw new InvalidOperationException("Cannot add items to a closed order.");
            }
            var orderItem = new OrderItem(this.Id, productId, orderChannel, note);
            _orderItems.Add(orderItem);
        }

        public void UpdateStatus(OrderStatus status)
        {
            Status = status;
        }

        public void Cancel()
        {
            if (Status == OrderStatus.Paid)
            {
                throw new InvalidOperationException("Cannot cancel a paid order.");
            }
            Status = OrderStatus.Cancelled;
        }

        public void Delete()
        {
            Status = OrderStatus.Deleted;
        }

        public void CheckOut()
        {
            if (_orderItems.Any(i => i.Status != OrderItemStatus.Served))
            {
                throw new InvalidOperationException("Cannot checkout. Some items are not served yet.");
            }
            Status = OrderStatus.Paid;
            AddDomainEvent(new OrderCheckedOutEvent(this.Id));
        }

        public decimal CalculateTotal()
        {
            return _orderItems.Sum(item => item.Product?.Price ?? 0);
        }
    }
}
