using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Domain.Enums
{
    public enum OrderStatus
    {
        Pending = 0,     // Chỉ dùng cho Delivery
        Confirmed = 1,   // Đã xác nhận đơn
        Preparing = 2,   // Đang chuẩn bị
        Delivering = 3,  // Đang giao (Delivery)
        Paying = 4,      // Đang thanh toán 
        Completed = 5,   // Hoàn tất
        Cancelled = 6    // Huỷ đơn
    }
}
