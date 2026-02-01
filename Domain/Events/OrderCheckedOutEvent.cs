using RestaurantOrderTracking.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Domain.Events
{
    public record OrderCheckedOutEvent(Guid OrderId) : IDomainEvent
    {
    }
}
