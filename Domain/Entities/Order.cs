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

        public Guid? CustomerId { get; private set; }
        public virtual Customer? Customer { get; private set; }

        public OrderStatus Status { get; private set; }

        private readonly List<OrderItem> _orderItems = new();
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

        public virtual Bill? Bill { get; private set; }

        protected Order() { }

        public Order(Guid tableId)
        {
            TableId = tableId;
            Status = OrderStatus.Open;
        }

        public void AddItem(Guid productId, Guid accountId, string note, string orderChannel)
        {
            if (Status == OrderStatus.Close || Status == OrderStatus.Paying)
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


        public decimal CalculateTotal()
        {
            return _orderItems.Sum(item => item.Product?.Price ?? 0);
        }
    }
}
