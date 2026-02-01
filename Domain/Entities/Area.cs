using RestaurantOrderTracking.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Domain.Entities
{
    public class Area : BaseEntities
    {
        public string Name { get; private set; } = null!;

        private readonly List<Table> Tables = new();
        public IReadOnlyCollection<Table> GetTables() => Tables.AsReadOnly();

        protected Area() { }

        public Area(string name)
        {
            Name = name;
        }

        public void UpdateName(string name)
        {
            Name = name;
        }
    }
}
