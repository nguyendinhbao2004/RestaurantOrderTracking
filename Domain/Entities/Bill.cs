using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Enums;
using System;

namespace RestaurantOrderTracking.Domain.Entities
{
    public class Bill : BaseEntities
    {
        public Guid OrderId { get; private set; }
        public virtual Order Order { get; private set; } = null!;

        public Guid? AccountId { get; private set; }
        public virtual Account Account { get; private set; } = null!;

        public decimal Amount { get; private set; }
        public decimal? Discount { get; private set; }
        public decimal FinalAmount { get; private set; }
        public float Tax { get; private set; }
        public PaymentMethod PaymentMethod { get; private set; }
        public BillStatus Status { get; private set; }
        public DateTime? PaidAt { get; private set; }
        public string? TransactionId { get; private set; }

        protected Bill() { }

        public Bill(Guid orderId, Guid? accountId, decimal amount, PaymentMethod paymentMethod, decimal? discount = null, float tax = 0)
        {
            OrderId = orderId;
            AccountId = accountId;
            Amount = amount;
            Discount = discount;
            Tax = tax;
            FinalAmount = amount * (decimal)(1 + tax) - (discount ?? 0);
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
            FinalAmount = Amount * (decimal)(1 + Tax) - discount;
        }

        public void Update(PaymentMethod? paymentMethod, decimal? discount)
        {
            if (Status != BillStatus.unpaid)
                throw new InvalidOperationException("Cannot update a bill that is not unpaid.");

            if (paymentMethod.HasValue)
                PaymentMethod = paymentMethod.Value;

            if (discount.HasValue)
            {
                Discount = discount.Value;
                FinalAmount = Amount * (decimal)(1 + Tax) - discount.Value;
            }
        }

        public void Refund()
        {
            if (Status != BillStatus.paid)
                throw new InvalidOperationException("Only paid bills can be refunded.");

            Status = BillStatus.refunded;
        }
    }
}
