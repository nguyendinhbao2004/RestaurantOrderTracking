using MediatR;
using RestaurantOrderTracking.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Application.Common.Model
{
    public class DomainEventNotification<TEvent> : INotification where TEvent : IDomainEvent
    {
        public TEvent DomainEvent { get; }

        public DomainEventNotification(TEvent domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}
