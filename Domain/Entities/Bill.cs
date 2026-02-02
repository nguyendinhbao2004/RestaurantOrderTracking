using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Enums;
using System;

namespace RestaurantOrderTracking.Domain.Entities
{
    public class Bill : BaseEntities
    {
        public Guid OrderId { get; private set; }
        public virtual Order Order { get; private set; } = null!;

        public Guid AccountId { get; private set; }
        public virtual Account Account { get; private set; } = null!;

        public decimal Amount { get; private set; }
        public decimal? Discount { get; private set; }
        public decimal FinalAmount { get; private set; }
        public PaymentMethod PaymentMethod { get; private set; }
        public BillStatus Status { get; private set; }
        public DateTime? PaidAt { get; private set; }
        public string? TransactionId { get; private set; }

        protected Bill() { }

        public Bill(Guid orderId, Guid accountId, decimal amount, PaymentMethod paymentMethod, decimal? discount = null)
        {
            OrderId = orderId;
            AccountId = accountId;
            Amount = amount;
            Discount = discount;
            FinalAmount = amount - (discount ?? 0);
            PaymentMethod = paymentMethod;
            Status = BillStatus.unpaid;
        }

        public void MarkAsPaid(string? transactionId = null)
        {
            Status = BillStatus.paid;
            PaidAt = DateTime.UtcNow;
            TransactionId = transactionId;
        }

        public void Cancel()
        {
            Status = BillStatus.cancelled;
        }

        public void ApplyDiscount(decimal discount)
        {
            Discount = discount;
            FinalAmount = Amount - discount;
        }
    }
}
