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

        public string SkillLevel { get; private set; } = null!;
        public bool IsAvailable { get; private set; }
        public int MaxTables { get; private set; }

        protected Waiter() { }

        public Waiter(Guid accountId)
        {
            AccountId = accountId;
        }

        public void UpdateWaiter(Guid assignedAreaId, string skillLevel, bool isAvailable, int maxTables)
        {
            AssignedAreaId = assignedAreaId;
            SkillLevel = skillLevel;
            IsAvailable = isAvailable;
            MaxTables = maxTables;
        }
    }
}
