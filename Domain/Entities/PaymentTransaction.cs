using RestaurantOrderTracking.Domain.Common;
using System;

namespace RestaurantOrderTracking.Domain.Entities
{
    public class PaymentTransaction : BaseEntities
    {
        public Guid BillId { get; private set; }
        public virtual Bill Bill { get; private set; } = null!;

        // PayOS uses orderCode as an integer up to 53 bits
        public long OrderCode { get; private set; }
        
        public decimal Amount { get; private set; }
        
        public string Status { get; private set; }
        
        public string? PaymentLinkId { get; private set; }

        protected PaymentTransaction() { }

        public PaymentTransaction(Guid billId, long orderCode, decimal amount, string status = "PENDING")
        {
            BillId = billId;
            OrderCode = orderCode;
            Amount = amount;
            Status = status;
        }

        public void UpdateStatus(string status)
        {
            Status = status;
        }

        public void SetPaymentLinkId(string paymentLinkId)
        {
            PaymentLinkId = paymentLinkId;
        }
    }
}
