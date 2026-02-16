using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Application.Dto.Table
{
    public class TableResponse
    {
        public Guid Id { get; set; }
        public string TableNumber { get; set; }
        public string AreaName { get; set; }
        public string Status { get; set; }
    }
}
