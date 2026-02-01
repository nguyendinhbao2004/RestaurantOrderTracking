using RestaurantOrderTracking.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Domain.Entities
{
    public class FeedBack : BaseEntities
    {
        public Guid OrderId { get; private set; }
        public virtual Order Orders { get; private set; }
        public int Rating { get; private set; }
        public string Comment { get; private set; }
    }
}
