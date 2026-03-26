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

        /// <summary>
        /// Used for special cases where status needs to be initialized to Ready (e.g. categoryId = 4)
        /// </summary>
        public void InitializeStatus(OrderItemStatus status)
        {
            if (Status == OrderItemStatus.Pending)
            {
                Status = status;
            }
        }

        /// <summary>
        /// Advances status to the next enum value in sequence, or to Cancelled.
        /// </summary>
        public void UpdateStatus(OrderItemStatus newStatus)
        {
            if (Status == OrderItemStatus.Cancelled)
                throw new InvalidOperationException("Cannot change status of a cancelled order item.");

            if (Status == OrderItemStatus.Served)
                throw new InvalidOperationException("Order item has already been served.");

            // Cancelled(6) only allowed from Pending(0) or Confirmed(1)
            if (newStatus == OrderItemStatus.Cancelled)
            {
                if (Status != OrderItemStatus.Pending && Status != OrderItemStatus.Confirmed)
                    throw new InvalidOperationException(
                        $"Order item can only be cancelled from Pending or Confirmed status. Current status: {Status}.");
                Status = OrderItemStatus.Cancelled;
                return;
            }

            // Enforce sequential progression
            var expectedNext = (OrderItemStatus)((int)Status + 1);
            if (newStatus != expectedNext)
                throw new InvalidOperationException(
                    $"Invalid status transition: cannot go from {Status} to {newStatus}. Expected next status: {expectedNext}.");

            Status = newStatus;
        }

        /// <summary>
        /// Assigns a chef when transitioning Confirmed → Cooking (1 → 2).
        /// </summary>
        public void AssignChef(Guid chefAccountId)
        {
            ChefAccountId = chefAccountId;
        }

        /// <summary>
        /// Assigns a waiter when transitioning Ready → Delivering (3 → 4).
        /// </summary>
        public void AssignWaiter(Guid waiterAccountId)
        {
            WaiterAccountId = waiterAccountId;
        }

        public void Cancel()
        {
            Status = OrderItemStatus.Cancelled;
        }

        public void UpdateNote(string? note)
        {
            Note = note;
        }

        /// <summary>
        /// Updates info fields with per-field status validation:
        /// - chefAccountId: only allowed when Status = Cooking (2)
        /// - waiterAccountId: only allowed when Status = Delivering (4)
        /// - note: only allowed when Status = Pending (0) or Confirmed (1)
        /// Pass null to skip updating a field.
        /// </summary>
        public void UpdateInfo(Guid? chefAccountId, Guid? waiterAccountId, string? note)
        {
            if (chefAccountId.HasValue)
            {
                if (Status != OrderItemStatus.Cooking)
                    throw new InvalidOperationException(
                        $"ChefId can only be updated when status is Cooking. Current status: {Status}.");
                ChefAccountId = chefAccountId.Value;
            }

            if (waiterAccountId.HasValue)
            {
                if (Status != OrderItemStatus.Delivering)
                    throw new InvalidOperationException(
                        $"WaiterId can only be updated when status is Delivering. Current status: {Status}.");
                WaiterAccountId = waiterAccountId.Value;
            }

            if (note is not null)
            {
                if (Status != OrderItemStatus.Pending && Status != OrderItemStatus.Confirmed)
                    throw new InvalidOperationException(
                        $"Note can only be updated when status is Pending or Confirmed. Current status: {Status}.");
                Note = note;
            }
        }
    }
}

