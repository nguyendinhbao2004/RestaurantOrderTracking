using RestaurantOrderTracking.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Domain.Entities
{
    public class Chef : BaseEntities
    {
        public Guid AccountId { get; private set; }
        public virtual Account Account { get; private set; } = null!;

        public string Specialty { get; private set; } = null!;
        public string SkillLevel { get; private set; } = null!;
        public bool IsAvailable { get; private set; }
        public string Station { get; private set; } = null!;

        protected Chef() { }

        public Chef(Guid accountId)
        {
            AccountId = accountId;
        }

        public void UpdateChef(string specialty, string skillLevel, bool isAvailable, string station)
        {
            Specialty = specialty;
            SkillLevel = skillLevel;
            IsAvailable = isAvailable;
            Station = station;
        }
    }
}
