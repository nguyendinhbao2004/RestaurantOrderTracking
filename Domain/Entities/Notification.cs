using RestaurantOrderTracking.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Domain.Entities
{
    public class Notification : BaseEntities
    {
        public Guid OrderItemId { get; private set; }
        public virtual OrderItems OrderItems { get; private set; }

        public Guid AccountId { get; private set; }
        public virtual Account Accounts { get; private set; }

        public Guid TableId { get; private set; }
        public virtual Table Tables { get; private set; }

        public string Title { get; private set; }

        public string Message { get; private set; }

        public DateTime ReadAt { get; private set; }

        public Notification(Guid orderItemId, Guid accountId, Guid tableId, string title, string message)
        {
            OrderItemId = orderItemId;
            AccountId = accountId;
            TableId = tableId;
            Title = title;
            Message = message;
        }

        public void MarkAsRead()
        {
            ReadAt = DateTime.UtcNow;
        }


    }
}
