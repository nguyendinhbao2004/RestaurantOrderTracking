using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Domain.Entities
{
    public class Bill : BaseEntities
    {
        public Guid OrderId { get; private set; }
        public virtual Order Orders { get; private set; }

        public Guid AccountId { get; private set; }
        public virtual Account Accounts { get; private set; }

        public decimal Amount { get; private set; }
        public PaymentMethod PaymentMethod { get; private set; }
        public BillStatus Status { get; private set; }
        public DateTime PaidAt { get; private set; } 
        public Bill(Guid orderId, Guid accountId, decimal amount, PaymentMethod paymentMethod)
        {
            OrderId = orderId;
            AccountId = accountId;
            Amount = amount;
            PaymentMethod = paymentMethod;
            PaidAt = DateTime.UtcNow;
            Status = BillStatus.unpaid;
        }

    }
}
