using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Enums;
using System;

namespace RestaurantOrderTracking.Domain.Entities
{
    public class Notification : BaseEntities
    {
        public Guid? OrderItemId { get; private set; }
        public virtual OrderItem? OrderItem { get; private set; }

        public Guid AccountId { get; private set; }
        public virtual Account Account { get; private set; } = null!;

        public Guid? TableId { get; private set; }
        public virtual Table? Table { get; private set; }

        public string Title { get; private set; } = null!;
        public string Message { get; private set; } = null!;
        public NotificationType Type { get; private set; }
        public bool IsRead { get; private set; }
        public DateTime? ReadAt { get; private set; }

        protected Notification() { }

        public Notification(Guid accountId, string title, string message, NotificationType type, Guid? orderItemId = null, Guid? tableId = null)
        {
            AccountId = accountId;
            Title = title;
            Message = message;
            Type = type;
            OrderItemId = orderItemId;
            TableId = tableId;
            IsRead = false;
        }

        public void MarkAsRead()
        {
            IsRead = true;
            ReadAt = DateTime.UtcNow;
        }

        public void MarkAsUnread()
        {
            IsRead = false;
            ReadAt = null;
        }
    }
}
