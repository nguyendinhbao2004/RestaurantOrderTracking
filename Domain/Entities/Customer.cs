using RestaurantOrderTracking.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Domain.Entities
{
    public class Customer : BaseEntities
    {
        public string Name { get; private set; } = null!;
        public string Phone { get; private set; } = null!;
        public string Address { get; private set; } = null!;

        private readonly List<Order> _orders = new();
        public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();

        protected Customer() { }

        public Customer(string name, string phone, string address)
        {
            Name = name;
            Phone = phone;
            Address = address;
        }

        public void UpdateInfo(string name, string phone, string address)
        {
            Name = name;
            Phone = phone;
            Address = address;
        }
    }
}
