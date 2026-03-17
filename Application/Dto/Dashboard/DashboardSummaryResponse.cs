using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.Dashboard
{
    public record DashboardSummaryResponse
    {
        public int TotalOrders { get; init; }
        public decimal TotalRevenue { get; init; }
        public decimal AvgOrderValue { get; init; }
        public int PendingOrders { get; init; }
    }
}
