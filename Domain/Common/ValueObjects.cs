using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Domain.Common
{
    public abstract class ValueObjects
    {
        protected static bool EqualOperator(ValueObjects left, ValueObjects right)
        {
            if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
            {
                return false;
            }
            return ReferenceEquals(left, null) || left.Equals(right);
        }

        protected static bool NotEqualOperator(ValueObjects left, ValueObjects right)
        {
            return !(EqualOperator(left, right));
        }

        // Hàm này bắt buộc các lớp con phải khai báo xem so sánh dựa trên thuộc tính nào
        protected abstract IEnumerable<object> GetEqualityComponents();
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }
            var other = (ValueObjects)obj;
            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }
        // Overload toán tử == và != để so sánh tiện hơn
        public static bool operator ==(ValueObjects left, ValueObjects right)
        {
            return EqualOperator(left, right);
        }
        public static bool operator !=(ValueObjects left, ValueObjects right)
        {
            return NotEqualOperator(left, right);
        }
    }
}
