using Domain.Enums;
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

        public ExpertiseChef Specialty { get; private set; } // Chuyên môn của đầu bếp
        public string SkillLevel { get; private set; } = null!;
        public bool IsAvailable { get; private set; }
        public string Station { get; private set; } = null!;

        protected Chef() { }

        public Chef(Guid accountId, ExpertiseChef specialty, string skillLevel, string station)
        {
            AccountId = accountId;
            Specialty = specialty;
            SkillLevel = skillLevel;
            IsAvailable = true;
            Station = station;
        }

        public void UpdateChef(ExpertiseChef specialty, string skillLevel, bool isAvailable, string station)
        {
            Specialty = specialty;
            SkillLevel = skillLevel;
            IsAvailable = isAvailable;
            Station = station;
        }
    }
}
