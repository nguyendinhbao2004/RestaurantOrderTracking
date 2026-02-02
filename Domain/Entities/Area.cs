using RestaurantOrderTracking.Domain.Common;
using System;
using System.Collections.Generic;

namespace RestaurantOrderTracking.Domain.Entities
{
    public class Area : BaseEntities
    {
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }

        private readonly List<Table> _tables = new();
        public IReadOnlyCollection<Table> Tables => _tables.AsReadOnly();

        protected Area() { }

        public Area(string name, string? description = null)
        {
            Name = name;
            Description = description;
        }

        public void UpdateName(string name)
        {
            Name = name;
        }

        public void UpdateDescription(string? description)
        {
            Description = description;
        }

        public void Update(string name, string? description)
        {
            Name = name;
            Description = description;
        }
    }
}
