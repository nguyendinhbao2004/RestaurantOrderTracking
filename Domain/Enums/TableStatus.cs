using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Domain.Enums
{
    public enum TableStatus
    {
        Available = 0, // bàn trống, có thể đặt hoặc phục vụ
        Occupied = 1, // đặt bàn trước
        Reserved = 2, // đang phục vụ 
        OutOfService = 3 // bàn hỏng, không sử dụng được
    }
}
