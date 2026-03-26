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
        public Guid? TableId { get; private set; }
        public virtual Table Table { get; private set; } = null!;

        public OrderType OrderTypes { get; private set; }

        public Guid? WaiterId { get; private set; }
        public virtual Account Waiter { get; private set; } = null!;

        public Guid? CustomerId { get; private set; }
        public virtual Customer? Customer { get; private set; }

        public OrderStatus Status { get; private set; }

        private readonly List<OrderItem> _orderItems = new();
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

        public virtual Bill? Bill { get; private set; }

        protected Order() { }

        public Order(Guid tableId, OrderType orderType, Guid? waiterId = null)
        {
            TableId = tableId;
            OrderTypes = orderType;
            WaiterId = waiterId;

            // NEW: set status khởi tạo theo type
            Status = orderType switch
            {
                OrderType.Delivery => OrderStatus.Pending,
                OrderType.DineIn => OrderStatus.Confirmed,
                OrderType.TakeAway => OrderStatus.Confirmed,
                _ => throw new ArgumentOutOfRangeException(nameof(orderType))
            };
        }

        /// <summary>
        /// Constructor dành riêng cho đơn hàng online (Delivery từ khách đặt tại nhà).
        /// TableId = null, CustomerId được gán ngay, WaiterId = null.
        /// </summary>
        public Order(OrderType orderType, Guid customerId)
        {
            TableId = null;
            CustomerId = customerId;
            OrderTypes = orderType;
            WaiterId = null;
            Status = OrderStatus.Pending;
        }

        public void AddItem(Guid productId, Guid? accountId, string note, string orderChannel, int? categoryId = null)
        {
            if (Status == OrderStatus.Paying || Status == OrderStatus.Completed || Status == OrderStatus.Cancelled)
            {
                throw new InvalidOperationException("Cannot add items to a closed order.");
            }
            var orderItem = new OrderItem(this.Id, productId, orderChannel, note);
            if (accountId.HasValue)
            {
                orderItem.CreatedBy = accountId.Value.ToString();
            }

            if (categoryId.HasValue && categoryId.Value == 4)
            {
                orderItem.InitializeStatus(OrderItemStatus.Ready); // Status = 3
            }

            _orderItems.Add(orderItem);
        }
        private bool IsValidTransition(OrderStatus newStatus)
        {
            return OrderTypes switch
            {
                OrderType.DineIn => IsValidDineInTransition(newStatus),
                OrderType.TakeAway => IsValidTakeAwayTransition(newStatus),
                OrderType.Delivery => IsValidDeliveryTransition(newStatus),
                _ => false
            };
        }
        private bool IsValidDineInTransition(OrderStatus newStatus)
        {
            return (Status, newStatus) switch
            {
                (OrderStatus.Confirmed, OrderStatus.Paying) => true,
                (OrderStatus.Paying, OrderStatus.Completed) => true,
                (_, OrderStatus.Cancelled) => true,
                _ => false
            };
        }
        private bool IsValidTakeAwayTransition(OrderStatus newStatus)
        {
            return (Status, newStatus) switch
            {
                (OrderStatus.Confirmed, OrderStatus.Preparing) => true,
                (OrderStatus.Preparing, OrderStatus.Paying) => true,
                (OrderStatus.Paying, OrderStatus.Completed) => true,
                _ => false
            };
        }
        private bool IsValidDeliveryTransition(OrderStatus newStatus)
        {
            return (Status, newStatus) switch
            {
                (OrderStatus.Pending, OrderStatus.Confirmed) => true,
                (OrderStatus.Confirmed, OrderStatus.Preparing) => true,
                (OrderStatus.Preparing, OrderStatus.Delivering) => true,
                (OrderStatus.Delivering, OrderStatus.Completed) => true,
                _ => false
            };
        }
        public void UpdateStatus(OrderStatus newStatus)
        {
            if (!IsValidTransition(newStatus))
            {
                throw new InvalidOperationException(
                    $"Invalid status transition from {Status} to {newStatus} for {OrderTypes}"
                );
            }

            Status = newStatus;
        }

        public void UpdateInfo(Guid newTableId, OrderType newOrderType)
        {
            if (Status == OrderStatus.Completed || Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("Cannot update a completed or cancelled order.");

            if (OrderTypes == OrderType.Delivery)
                throw new InvalidOperationException("Cannot update info of a delivery order.");

            if (newOrderType == OrderType.Delivery)
                throw new InvalidOperationException("Order type can only be DineIn or TakeAway.");

            TableId = newTableId;
            OrderTypes = newOrderType;
        }



        public void CheckOut()
        {
            if (_orderItems.Any(i => i.Status != OrderItemStatus.Served))
            {
                throw new InvalidOperationException("Cannot checkout. Some items are not served yet.");
            }

            
            UpdateStatus(OrderStatus.Paying);

            AddDomainEvent(new OrderCheckedOutEvent(this.Id));
        }

        public decimal CalculateTotal()
        {
            return _orderItems.Sum(item => item.Product?.Price ?? 0);
        }
    }
}
