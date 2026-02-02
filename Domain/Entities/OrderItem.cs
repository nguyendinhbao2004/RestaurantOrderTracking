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
        public int Quantity { get; private set; } = 1;
        public decimal UnitPrice { get; private set; }
        public OrderItemStatus Status { get; private set; }

        public DateTime? ConfirmedAt { get; private set; }
        public DateTime? KitchenConfirmedAt { get; private set; }
        public DateTime? KitchenFinishedAt { get; private set; }
        public DateTime? WaiterArrivedAt { get; private set; }
        public DateTime? WaiterServedAt { get; private set; }

        protected OrderItem() { }

        public OrderItem(Guid orderId, Guid productId, string orderChannel, string? note = null, int quantity = 1)
        {
            OrderId = orderId;
            ProductId = productId;
            OrderChannel = orderChannel;
            Note = note;
            Quantity = quantity;
            Status = OrderItemStatus.Pending;
        }

        public void Confirm()
        {
            Status = OrderItemStatus.Confirmed;
            ConfirmedAt = DateTime.UtcNow;
        }

        public void ConfirmByKitchen(Guid chefId)
        {
            ChefAccountId = chefId;
            Status = OrderItemStatus.Cooking;
            KitchenConfirmedAt = DateTime.UtcNow;
        }

        public void FinishByKitchen()
        {
            Status = OrderItemStatus.Ready;
            KitchenFinishedAt = DateTime.UtcNow;
        }

        public void PickUpByWaiter(Guid waiterId)
        {
            WaiterAccountId = waiterId;
            Status = OrderItemStatus.Delivering;
            WaiterArrivedAt = DateTime.UtcNow;
        }

        public void ServeByWaiter()
        {
            Status = OrderItemStatus.Served;
            WaiterServedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            Status = OrderItemStatus.Cancelled;
        }

        public void UpdateNote(string? note)
        {
            Note = note;
        }

        public void UpdateQuantity(int quantity)
        {
            if (quantity < 1)
            {
                throw new ArgumentException("Quantity must be at least 1.", nameof(quantity));
            }
            Quantity = quantity;
        }

        public void SetUnitPrice(decimal unitPrice)
        {
            UnitPrice = unitPrice;
        }

        public decimal GetTotalPrice()
        {
            return UnitPrice * Quantity;
        }
    }
}
