using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Domain.Enums
{
    public enum OrderStatus
    {
        Pending = 0,
        InPreparation = 1,
        Ready = 2,
        Served = 3,
        Cancelled = 4,
        Paid = 5,
        Deleted = 6
    }
}
