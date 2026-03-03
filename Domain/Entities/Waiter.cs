using RestaurantOrderTracking.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Domain.Entities
{
    public class Waiter : BaseEntities
    {
        public Guid AccountId { get; private set; }
        public virtual Account Account { get; private set; } = null!;

        public Guid AssignedAreaId { get; private set; }
        public virtual Area AssignedArea { get; private set; } = null!;
        public int? OrderCount { get; private set; }

        protected Waiter() { }

        public Waiter(Guid accountId, Guid assignedAreaId, bool isAvailable, int? orderCount)
        {
            AccountId = accountId;
            AssignedAreaId = assignedAreaId;
            OrderCount = orderCount;
        }

        public void UpdateWaiter(Guid assignedAreaId, bool isAvailable, int? orderCount)
        {
            AssignedAreaId = assignedAreaId;
            OrderCount = orderCount;
        }
    }
}
