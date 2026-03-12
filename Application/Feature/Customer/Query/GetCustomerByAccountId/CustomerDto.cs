using System;
using System.Collections.Generic;
using RestaurantOrderTracking.Domain.Entities;

namespace Application.Feature.Customers.Query.GetCustomerByAccountId
{
    public class CustomerDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Address { get; set; } = null!;
    }
}
