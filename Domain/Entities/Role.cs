using RestaurantOrderTracking.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Domain.Entities
{
    public class Role 
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = null!;

        public string Decription { get; private set; } = null!;

        protected Role() { }

        public Role(int id, string name, string decription)
        {
            Id = id;
            Name = name;
            Decription = decription;
        }
        public void UpdateRole(string name, string decription)
        {
            Name = name;
            Decription = decription;
        }
    }
}
