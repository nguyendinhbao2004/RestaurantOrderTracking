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


        protected Chef() { }

        public Chef(Guid accountId, ExpertiseChef specialty, string skillLevel)
        {
            AccountId = accountId;
            Specialty = specialty;
            SkillLevel = skillLevel;
            IsAvailable = true;
        }

        public void UpdateChef(ExpertiseChef specialty, string skillLevel, bool isAvailable)
        {
            Specialty = specialty;
            SkillLevel = skillLevel;
            IsAvailable = isAvailable;
        }
    }
}
