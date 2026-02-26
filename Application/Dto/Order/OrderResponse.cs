using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Application.Dto.Order
{
    public class OrderResponse
    {
        public int Id { get; set; }
        public string TableNumber { get; set; }
        public string Status { get; set; }
    }
}
