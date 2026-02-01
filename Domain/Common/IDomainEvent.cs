using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Domain.Common
{
    public interface IDomainEvent : INotification
    {
    }
}
